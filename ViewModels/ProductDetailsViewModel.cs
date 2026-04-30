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
        set => SetProperty(ref _product, value);
    }

    private int _quantity = 1;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value >= 1 && value <= 10)
                SetProperty(ref _quantity, value);
        }
    }

    public ICommand AddToCartCommand { get; }
    public ICommand IncreaseQuantityCommand { get; }
    public ICommand DecreaseQuantityCommand { get; }

    public ProductDetailsViewModel(CartService cartService)
    {
        _cartService = cartService;
        AddToCartCommand = new Command(OnAddToCart);
        IncreaseQuantityCommand = new Command(() => Quantity++);
        DecreaseQuantityCommand = new Command(() => Quantity--);
    }

    private async void OnAddToCart()
    {
        // Actually add the product to the global cart service
        await _cartService.AddProductAsync(Product, Quantity);

        MessagingCenter.Send(this, "ItemAddedToCart");
        
        // Optional: Navigate back or stay on page
        // await Shell.Current.GoToAsync("..");
    }
}
