using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using E_Commerce.Models;
using E_Commerce.Services;

namespace E_Commerce.ViewModels;

public class CartViewModel : BaseViewModel
{
    private readonly CartService _cartService;
    public ObservableCollection<CartItem> CartItems => _cartService.CartItems;

    private decimal _subtotal;
    public decimal Subtotal
    {
        get => _subtotal;
        set => SetProperty(ref _subtotal, value);
    }

    private decimal _shipping;
    public decimal Shipping
    {
        get => _shipping;
        set => SetProperty(ref _shipping, value);
    }

    private decimal _totalAmount;
    public decimal TotalAmount
    {
        get => _totalAmount;
        set => SetProperty(ref _totalAmount, value);
    }

    private int _itemCount;
    public int ItemCount
    {
        get => _itemCount;
        set => SetProperty(ref _itemCount, value);
    }

    private bool _isCartEmpty;
    public bool IsCartEmpty
    {
        get => _isCartEmpty;
        set => SetProperty(ref _isCartEmpty, value);
    }

    public ICommand RemoveItemCommand { get; }
    public ICommand IncreaseCommand { get; }
    public ICommand DecreaseCommand { get; }
    public ICommand CheckoutCommand { get; }
    public ICommand GoBackCommand { get; }

    public CartViewModel(CartService cartService)
    {
        _cartService = cartService;
        Title = "My Cart";
        
        // Subscribe to collection changes to update totals
        CartItems.CollectionChanged += (s, e) => UpdateTotals();
        
        RemoveItemCommand = new Command<CartItem>(OnRemoveItem);
        IncreaseCommand = new Command<CartItem>(OnIncrease);
        DecreaseCommand = new Command<CartItem>(OnDecrease);
        CheckoutCommand = new Command(OnCheckout);
        GoBackCommand = new Command(OnGoBack);
        
        UpdateTotals();
    }

    private async void OnGoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnRemoveItem(CartItem item)
    {
        await _cartService.RemoveItemAsync(item);
        UpdateTotals();
    }

    private async void OnIncrease(CartItem item)
    {
        if (item.Quantity < 10)
        {
            await _cartService.UpdateQuantityAsync(item, item.Quantity + 1);
            UpdateTotals();
        }
    }

    private async void OnDecrease(CartItem item)
    {
        if (item.Quantity > 1)
        {
            await _cartService.UpdateQuantityAsync(item, item.Quantity - 1);
            UpdateTotals();
        }
        else
        {
            OnRemoveItem(item);
        }
    }

    private async void OnCheckout()
    {
        await Shell.Current.GoToAsync(nameof(E_Commerce.Views.CheckoutPage));
    }

    private void UpdateTotals()
    {
        Subtotal = CartItems.Sum(x => x.TotalPrice);
        ItemCount = CartItems.Sum(x => x.Quantity);
        Shipping = (Subtotal > 100 || Subtotal == 0) ? 0m : 9.99m;
        TotalAmount = Subtotal + Shipping;
        IsCartEmpty = !CartItems.Any();
    }
}
