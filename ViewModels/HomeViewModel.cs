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

    public ICommand NavigateToCartCommand { get; }
    public ICommand SelectCategoryCommand { get; }
    public ICommand ProductSelectedCommand { get; }
    public ICommand ToggleCategoriesCommand { get; }
    private bool _showCategories = true;
    public bool ShowCategories
    {
        get => _showCategories;
        set => SetProperty(ref _showCategories, value);
    }

    private List<Product> _allProducts = new();

    public HomeViewModel()
    {
        Title = "Home";
        if (AuthService.CurrentUser != null)
            UserName = AuthService.CurrentUser.FullName.Split(' ')[0];

        LoadData();
        NavigateToCartCommand = new Command(NavigateToCart);
        // Initialize other commands
        SelectCategoryCommand = new Command<Category>(OnCategorySelected);
        ProductSelectedCommand = new Command<Product>(OnProductSelected);
        ToggleCategoriesCommand = new Command(ToggleCategories);
        ShowCategories = true;
    }

    private void LoadData()
    {
        Categories.Clear();
        Categories.Add(new Category { Name = "All", Icon = "🏷️", IsSelected = true });
        Categories.Add(new Category { Name = "Shoes", Icon = "👟" });
        Categories.Add(new Category { Name = "Clothing", Icon = "👕" });
        Categories.Add(new Category { Name = "Electronics", Icon = "💻" });
        Categories.Add(new Category { Name = "Watches", Icon = "⌚" });
        Categories.Add(new Category { Name = "Bags", Icon = "👜" });
        Categories.Add(new Category { Name = "Sports", Icon = "⚽" });

        _allProducts = new List<Product>
        {
            new Product
            {
                Id = 1, Name = "Nike Air Max 270", Price = 119.99m, OriginalPrice = 150.00m,
                Image = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400",
                Description = "The Nike Air Max 270 delivers visible cushioning under every step. Updated for modern comfort with a large window and fresh colors.",
                Rating = 4.5, ReviewCount = 2847, Category = "Shoes", Badge = "SALE"
            },
            new Product
            {
                Id = 2, Name = "Leather Jacket", Price = 249.99m, OriginalPrice = 249.99m,
                Image = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400",
                Description = "Premium genuine leather jacket with a modern slim fit. Features quality YKK zippers, inner pockets, and soft quilted lining.",
                Rating = 4.8, ReviewCount = 1204, Category = "Clothing", Badge = "HOT"
            },
            new Product
            {
                Id = 3, Name = "Smart Watch Pro", Price = 299.00m, OriginalPrice = 399.00m,
                Image = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400",
                Description = "Heart rate monitoring, GPS tracking, sleep analysis, and a stunning always-on AMOLED display with 5-day battery life.",
                Rating = 4.7, ReviewCount = 3412, Category = "Watches", Badge = "SALE"
            },
            new Product
            {
                Id = 4, Name = "Wireless Headphones", Price = 279.50m, OriginalPrice = 349.00m,
                Image = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400",
                Description = "Industry-leading noise canceling with Auto NC Optimizer. Crystal clear hands-free calling. Up to 30hrs battery life.",
                Rating = 4.9, ReviewCount = 5621, Category = "Electronics", Badge = "SALE"
            },
            new Product
            {
                Id = 5, Name = "Running Sneakers", Price = 89.99m, OriginalPrice = 89.99m,
                Image = "https://images.unsplash.com/photo-1491553895911-0055eca6402d?w=400",
                Description = "Lightweight performance running shoes with responsive cushioning and breathable mesh upper for daily runs.",
                Rating = 4.3, ReviewCount = 876, Category = "Shoes", Badge = "NEW"
            },
            new Product
            {
                Id = 6, Name = "Designer Backpack", Price = 79.99m, OriginalPrice = 99.99m,
                Image = "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=400",
                Description = "Water-resistant designer backpack with padded laptop compartment, multiple organizer pockets, and ergonomic straps.",
                Rating = 4.4, ReviewCount = 1532, Category = "Bags", Badge = "SALE"
            },
            new Product
            {
                Id = 7, Name = "Denim Jacket", Price = 69.99m, OriginalPrice = 69.99m,
                Image = "https://images.unsplash.com/photo-1576995853123-5a10305d93c0?w=400",
                Description = "Classic denim jacket with a vintage wash. Perfect layering piece for any season with comfortable stretch fabric.",
                Rating = 4.2, ReviewCount = 643, Category = "Clothing"
            },
            new Product
            {
                Id = 8, Name = "Bluetooth Speaker", Price = 49.99m, OriginalPrice = 79.99m,
                Image = "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=400",
                Description = "Portable waterproof speaker with 360-degree sound, 12-hour battery, and built-in microphone for calls.",
                Rating = 4.6, ReviewCount = 2190, Category = "Electronics", Badge = "SALE"
            },
            new Product
            {
                Id = 9, Name = "Fitness Tracker", Price = 59.99m, OriginalPrice = 59.99m,
                Image = "https://images.unsplash.com/photo-1575311373937-040b8e1fd5b6?w=400",
                Description = "Track steps, heart rate, sleep, and 20+ workout modes. Water resistant with 7-day battery and color display.",
                Rating = 4.1, ReviewCount = 987, Category = "Watches", Badge = "NEW"
            },
            new Product
            {
                Id = 10, Name = "Canvas Tote Bag", Price = 34.99m, OriginalPrice = 44.99m,
                Image = "https://images.unsplash.com/photo-1544816155-12df9643f363?w=400",
                Description = "Eco-friendly canvas tote with reinforced handles, inner zip pocket, and magnetic closure. Perfect for daily use.",
                Rating = 4.5, ReviewCount = 756, Category = "Bags", Badge = "SALE"
            },
        };

        SearchTerm = string.Empty;
        ApplyFilters();
    }

    private void OnCategorySelected(Category category)
    {
        foreach (var c in Categories)
            c.IsSelected = false;
        category.IsSelected = true;

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        // Apply combined filters for category and search term (case‑insensitive contains)
        var selectedCategory = Categories.FirstOrDefault(c => c.IsSelected)?.Name ?? "All";
        var term = SearchTerm?.Trim();

        IEnumerable<Product> filtered = _allProducts;

        if (selectedCategory != "All")
            filtered = filtered.Where(p => p.Category == selectedCategory);

        if (!string.IsNullOrWhiteSpace(term))
            filtered = filtered.Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                                    || p.Description.Contains(term, StringComparison.OrdinalIgnoreCase));

        Products.Clear();
        foreach (var p in filtered)
            Products.Add(p);

    }

    private async void OnProductSelected(Product product)
    {
        if (product == null) return;
        await Shell.Current.GoToAsync($"{nameof(ProductDetailsPage)}", new Dictionary<string, object>
        {
            { "Product", product }
        });
    }

    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (SetProperty(ref _searchTerm, value))
                ApplyFilters();
        }
    }

    private void NavigateToCart()
    {
        // Navigate to the Cart tab/page (use absolute path for Shell tabs)
        Shell.Current.GoToAsync("//CartPage");
    }

    private void ToggleCategories()
    {
        ShowCategories = !ShowCategories;
    }
}
