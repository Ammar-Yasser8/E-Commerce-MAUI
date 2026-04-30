using System.Windows.Input;
using E_Commerce.Services;
using E_Commerce.Views;

namespace E_Commerce.ViewModels;

public class AdminDashboardViewModel : BaseViewModel
{
    public ICommand NavigateToProductsCommand { get; }
    public ICommand NavigateToCategoriesCommand { get; }
    public ICommand NavigateToOrdersCommand { get; }
    public ICommand GoBackCommand { get; }

    public AdminDashboardViewModel()
    {
        Title = "Admin Dashboard";

        NavigateToProductsCommand = new Command(async () => await Shell.Current.GoToAsync("AdminProductsPage"));
        NavigateToCategoriesCommand = new Command(async () => await Shell.Current.GoToAsync("AdminCategoriesPage"));
        NavigateToOrdersCommand = new Command(async () => await Shell.Current.GoToAsync("AdminOrdersPage"));
        GoBackCommand = new Command(async () => await Shell.Current.GoToAsync("//HomePage"));
    }
}
