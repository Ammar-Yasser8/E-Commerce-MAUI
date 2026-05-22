using E_Commerce.Models;
using E_Commerce.Services;
using System.Windows.Input;

namespace E_Commerce.ViewModels;

[QueryProperty(nameof(Product), "Product")]
public class ProductDetailsViewModel : BaseViewModel
{
    private readonly CartService _cartService;
    private Product _product = new();
    public Product Product
    {
        get => _product;
        set
        {
            if (SetProperty(ref _product, value))
            {
                OnPropertyChanged(nameof(IsFavorite));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
    }

    private int _quantity = 1;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value >= 1 && value <= 10)
            {
                if (SetProperty(ref _quantity, value))
                {
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }
    }

    public bool IsFavorite => Product?.IsFavorite ?? false;
    public decimal TotalPrice => Product != null ? Product.Price * Quantity : 0.0m;

    public bool IsInCart => _cartService.CartItems.Any(x => x.Product.Id == Product?.Id);
    public string CartButtonText => IsInCart ? "Go to Cart  ➡️" : "Add to Cart  🛒";
    public int CartItemsCount => _cartService.CartItems.Sum(x => x.Quantity);
    public bool HasCartItems => CartItemsCount > 0;

    public ICommand AddToCartCommand { get; }
    public ICommand IncreaseQuantityCommand { get; }
    public ICommand DecreaseQuantityCommand { get; }
    public ICommand GoBackCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand NavigateToCartCommand { get; }

    public ProductDetailsViewModel(CartService cartService)
    {
        _cartService = cartService;
        AddToCartCommand = new Command(OnAddToCart);
        IncreaseQuantityCommand = new Command(() => Quantity++);
        DecreaseQuantityCommand = new Command(() => Quantity--);
        GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        ToggleFavoriteCommand = new Command(OnToggleFavorite);
        NavigateToCartCommand = new Command(async () => await Shell.Current.GoToAsync("//CartPage"));
    }

    public void OnAppearing()
    {
        _cartService.CartItems.CollectionChanged += OnCartItemsChanged;
        UpdateCartStatus();
    }

    public void OnDisappearing()
    {
        _cartService.CartItems.CollectionChanged -= OnCartItemsChanged;
    }

    private void OnCartItemsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        UpdateCartStatus();
    }

    private void UpdateCartStatus()
    {
        OnPropertyChanged(nameof(IsInCart));
        OnPropertyChanged(nameof(CartButtonText));
        OnPropertyChanged(nameof(CartItemsCount));
        OnPropertyChanged(nameof(HasCartItems));
    }

    private void OnToggleFavorite()
    {
        if (Product != null)
        {
            Product.IsFavorite = !Product.IsFavorite;
            OnPropertyChanged(nameof(IsFavorite));
            OnPropertyChanged(nameof(Product));
        }
    }

    private async void OnAddToCart()
    {
        if (IsInCart)
        {
            await Shell.Current.GoToAsync("//CartPage");
            return;
        }

        // Actually add the product to the global cart service
        await _cartService.AddProductAsync(Product, Quantity);

        MessagingCenter.Send(this, "ItemAddedToCart");
        UpdateCartStatus();
    }
}
