using System.Collections.ObjectModel;
using System.Windows.Input;
using E_Commerce.Models;
using E_Commerce.Services;
using E_Commerce.Views;

namespace E_Commerce.ViewModels;

public class HomeViewModel : BaseViewModel
{
    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<Category> Categories { get; } = new();

    private string _userName = "there";
    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    private bool _isAdmin;
    public bool IsAdmin
    {
        get => _isAdmin;
        set => SetProperty(ref _isAdmin, value);
    }

    public ICommand NavigateToCartCommand { get; }
    public ICommand NavigateToAdminCommand { get; }
    public ICommand SelectCategoryCommand { get; }
    public ICommand ProductSelectedCommand { get; }
    public ICommand ToggleCategoriesCommand { get; }
    public ICommand LoadMoreProductsCommand { get; }

    private bool _showCategories = true;
    public bool ShowCategories
    {
        get => _showCategories;
        set => SetProperty(ref _showCategories, value);
    }

    private readonly ApiService _apiService;
    private int _pageIndex = 1;
    private const int _pageSize = 10;
    private bool _hasMoreProducts = true;
    private bool _isLoadingMore = false;

    public HomeViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Title = "Home";
        if (AuthService.CurrentUser != null)
        {
            UserName = AuthService.CurrentUser.FullName.Split(' ')[0];
            IsAdmin = AuthService.CurrentUser.Role == "Admin";
        }

        NavigateToCartCommand = new Command(NavigateToCart);
        NavigateToAdminCommand = new Command(async () => await Shell.Current.GoToAsync("AdminDashboardPage"));
        SelectCategoryCommand = new Command<Category>(OnCategorySelected);
        ProductSelectedCommand = new Command<Product>(OnProductSelected);
        ToggleCategoriesCommand = new Command(ToggleCategories);
        LoadMoreProductsCommand = new Command(async () => await LoadMoreProductsAsync());
        
        ShowCategories = true;

        Task.Run(LoadDataAsync);
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;

        var cats = await _apiService.GetCategoriesAsync();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Categories.Clear();
            Categories.Add(new Category { Id = 0, Name = "All", Icon = "🏷️", IsSelected = true });
            if (cats != null)
            {
                foreach (var c in cats)
                {
                    Categories.Add(new Category { Id = c.Id, Name = c.Name, Icon = "📂" });
                }
            }
        });

        await FetchProductsAsync(reset: true);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsBusy = false;
        });
    }

    private async Task FetchProductsAsync(bool reset)
    {
        if (reset)
        {
            _pageIndex = 1;
            _hasMoreProducts = true;
            MainThread.BeginInvokeOnMainThread(() => Products.Clear());
        }

        if (!_hasMoreProducts) return;

        int? catId = null;
        var selectedCat = Categories.FirstOrDefault(c => c.IsSelected);
        if (selectedCat != null && selectedCat.Id != 0)
        {
            catId = selectedCat.Id;
        }

        var term = SearchTerm?.Trim();

        var prods = await _apiService.GetProductsAsync(catId, null, term, _pageIndex, _pageSize);

        if (prods != null && prods.Data != null && prods.Data.Count > 0)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var p in prods.Data)
                {
                    Products.Add(new Product
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        OriginalPrice = p.Price * 1.2m, // mock
                        Image = string.IsNullOrEmpty(p.PictureUrl) || p.PictureUrl.StartsWith("http") 
                                ? p.PictureUrl 
                                : $"https://mauiapp.runasp.net/{p.PictureUrl.TrimStart('/')}",
                        Category = p.Category,
                        Rating = 4.5,
                        ReviewCount = 100
                    });
                }
            });

            // Compare loaded count vs total count, or if we received fewer than pageSize, we know we're at the end.
            if (prods.Data.Count < _pageSize)
            {
                _hasMoreProducts = false;
            }
        }
        else
        {
            _hasMoreProducts = false;
        }
    }

    private async Task LoadMoreProductsAsync()
    {
        if (_isLoadingMore || !_hasMoreProducts || IsBusy) return;

        _isLoadingMore = true;
        _pageIndex++;
        await FetchProductsAsync(reset: false);
        _isLoadingMore = false;
    }

    private async void OnCategorySelected(Category category)
    {
        foreach (var c in Categories)
            c.IsSelected = false;
        category.IsSelected = true;

        IsBusy = true;
        await FetchProductsAsync(reset: true);
        IsBusy = false;
    }

    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (SetProperty(ref _searchTerm, value))
            {
                // Fetch immediately (could be debounced later if needed)
                Task.Run(async () => 
                {
                    IsBusy = true;
                    await FetchProductsAsync(reset: true);
                    IsBusy = false;
                });
            }
        }
    }

    private async void OnProductSelected(Product product)
    {
        if (product == null) return;
        await Shell.Current.GoToAsync($"{nameof(ProductDetailsPage)}", new Dictionary<string, object>
        {
            { "Product", product }
        });
    }

    private void NavigateToCart()
    {
        Shell.Current.GoToAsync("//CartPage");
    }

    private void ToggleCategories()
    {
        ShowCategories = !ShowCategories;
    }
}
