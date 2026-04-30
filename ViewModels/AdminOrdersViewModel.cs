using System.Collections.ObjectModel;
using System.Windows.Input;
using E_Commerce.Models.Api;
using E_Commerce.Services;

namespace E_Commerce.ViewModels;

public class AdminOrdersViewModel : BaseViewModel
{
    private readonly ApiService _apiService;
    public ObservableCollection<OrderDto> Orders { get; } = new();

    public ICommand LoadOrdersCommand { get; }
    public ICommand UpdateStatusCommand { get; }
    public ICommand GoBackCommand { get; }

    public AdminOrdersViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Title = "Manage Orders";

        LoadOrdersCommand = new Command(async () => await LoadOrdersAsync());
        UpdateStatusCommand = new Command<OrderDto>(OnUpdateStatus);
        GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        Task.Run(LoadOrdersAsync);
    }

    private async Task LoadOrdersAsync()
    {
        IsBusy = true;
        var orders = await _apiService.GetAllOrdersAsync();
        if (orders != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Orders.Clear();
                foreach (var o in orders)
                    Orders.Add(o);
            });
        }
        IsBusy = false;
    }

    private async void OnUpdateStatus(OrderDto order)
    {
        if (order == null) return;

        if (App.Current?.MainPage != null)
        {
            string newStatus = await App.Current.MainPage.DisplayActionSheet("Update Status", "Cancel", null, "Pending", "Shipped", "Delivered", "Cancelled");
            if (string.IsNullOrEmpty(newStatus) || newStatus == "Cancel") return;

            IsBusy = true;
            bool success = await _apiService.UpdateOrderStatusAsync(order.Id, newStatus);
            IsBusy = false;

            if (success)
            {
                await LoadOrdersAsync();
                await App.Current.MainPage.DisplayAlert("Success", "Order status updated.", "OK");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Failed to update order status.", "OK");
            }
        }
    }
}
