using System.Collections.ObjectModel;
using System.Windows.Input;
using E_Commerce.Models.Api;
using E_Commerce.Services;

namespace E_Commerce.ViewModels;

public class AdminCategoriesViewModel : BaseViewModel
{
    private readonly ApiService _apiService;
    public ObservableCollection<ProductCategory> Categories { get; } = new();

    public ICommand LoadCategoriesCommand { get; }
    public ICommand CreateCategoryCommand { get; }
    public ICommand EditCategoryCommand { get; }
    public ICommand GoBackCommand { get; }

    public AdminCategoriesViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Title = "Manage Categories";

        LoadCategoriesCommand = new Command(async () => await LoadCategoriesAsync());
        CreateCategoryCommand = new Command(async () => await Shell.Current.GoToAsync("CategoryEditPage"));
        EditCategoryCommand = new Command<ProductCategory>(async (cat) => await OnEditCategory(cat));
        GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        Task.Run(LoadCategoriesAsync);
    }

    private async Task OnEditCategory(ProductCategory category)
    {
        if (category == null) return;
        var parameters = new Dictionary<string, object> { { "Category", category } };
        await Shell.Current.GoToAsync("CategoryEditPage", parameters);
    }

    private async Task LoadCategoriesAsync()
    {
        IsBusy = true;
        var cats = await _apiService.GetCategoriesAsync();
        if (cats != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Categories.Clear();
                foreach (var c in cats)
                    Categories.Add(c);
            });
        }
        IsBusy = false;
    }
}
