using System.Collections.ObjectModel;
using System.Windows.Input;
using E_Commerce.Models.Api;
using E_Commerce.Services;

namespace E_Commerce.ViewModels;

public class AdminProductsViewModel : BaseViewModel
{
    private readonly ApiService _apiService;
    public ObservableCollection<ProductToReverseDto> Products { get; } = new();

    public ICommand LoadProductsCommand { get; }
    public ICommand LoadMoreProductsCommand { get; }
    public ICommand DeleteProductCommand { get; }
    public ICommand CreateProductCommand { get; }
    public ICommand EditProductCommand { get; }
    public ICommand GoBackCommand { get; }

    private int _pageIndex = 1;
    private const int _pageSize = 10;
    private bool _hasMoreProducts = true;
    private bool _isLoadingMore = false;

    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (SetProperty(ref _searchTerm, value))
            {
                Task.Run(async () => await LoadProductsAsync(reset: true));
            }
        }
    }

    public AdminProductsViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Title = "Manage Products";

        LoadProductsCommand = new Command(async () => await LoadProductsAsync(reset: true));
        LoadMoreProductsCommand = new Command(async () => await LoadMoreProductsAsync());
        DeleteProductCommand = new Command<ProductToReverseDto>(OnDeleteProduct);
        CreateProductCommand = new Command(async () => await Shell.Current.GoToAsync("ProductEditPage"));
        EditProductCommand = new Command<ProductToReverseDto>(async (prod) => await OnEditProduct(prod));
        GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        Task.Run(async () => await LoadProductsAsync(reset: true));
    }

    private async Task OnEditProduct(ProductToReverseDto product)
    {
        if (product == null) return;
        var parameters = new Dictionary<string, object> { { "Product", product } };
        await Shell.Current.GoToAsync("ProductEditPage", parameters);
    }

    private async Task LoadProductsAsync(bool reset)
    {
        if (reset)
        {
            _pageIndex = 1;
            _hasMoreProducts = true;
            MainThread.BeginInvokeOnMainThread(() => Products.Clear());
        }

        if (!_hasMoreProducts || IsBusy) return;

        IsBusy = true;
        var prods = await _apiService.GetProductsAsync(null, null, string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm.Trim(), _pageIndex, _pageSize); 
        if (prods != null && prods.Data != null && prods.Data.Count > 0)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var p in prods.Data)
                {
                    // Prepend base URL if relative path
                    if (!string.IsNullOrEmpty(p.PictureUrl) && !p.PictureUrl.StartsWith("http"))
                    {
                        p.PictureUrl = $"https://mauiapp.runasp.net/{p.PictureUrl.TrimStart('/')}";
                    }
                    Products.Add(p);
                }
            });

            if (prods.Data.Count < _pageSize)
            {
                _hasMoreProducts = false;
            }
        }
        else
        {
            _hasMoreProducts = false;
        }
        IsBusy = false;
    }

    private async Task LoadMoreProductsAsync()
    {
        if (_isLoadingMore || !_hasMoreProducts || IsBusy) return;

        _isLoadingMore = true;
        _pageIndex++;
        await LoadProductsAsync(reset: false);
        _isLoadingMore = false;
    }

    private async void OnDeleteProduct(ProductToReverseDto product)
    {
        if (product == null) return;

        if (App.Current?.MainPage != null)
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirm Delete", $"Delete {product.Name}?", "Yes", "No");
            if (!confirm) return;

            IsBusy = true;
            bool success = await _apiService.DeleteProductAsync(product.Id);
            IsBusy = false;

            if (success)
            {
                Products.Remove(product);
                await App.Current.MainPage.DisplayAlert("Success", "Product deleted successfully.", "OK");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Failed to delete product.", "OK");
            }
        }
    }
}
