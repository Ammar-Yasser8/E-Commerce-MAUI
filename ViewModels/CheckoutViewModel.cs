using System.Windows.Input;
using E_Commerce.Services;

namespace E_Commerce.ViewModels;

public class CheckoutViewModel : BaseViewModel
{
    private readonly CartService _cartService;
    private readonly ApiService _apiService;

    public decimal Subtotal => _cartService.CartItems.Sum(x => x.TotalPrice);
    public decimal Shipping => (Subtotal > 100 || Subtotal == 0) ? 0m : 9.99m;
    public decimal TotalAmount => Subtotal + Shipping;

    private string _paymentIntentId = string.Empty;
    public string PaymentIntentId
    {
        get => _paymentIntentId;
        set => SetProperty(ref _paymentIntentId, value);
    }

    private string _clientSecret = string.Empty;
    public string ClientSecret
    {
        get => _clientSecret;
        set => SetProperty(ref _clientSecret, value);
    }

    private string _cardNumber = string.Empty;
    public string CardNumber
    {
        get => _cardNumber;
        set => SetProperty(ref _cardNumber, value);
    }

    private string _expiryDate = string.Empty;
    public string ExpiryDate
    {
        get => _expiryDate;
        set => SetProperty(ref _expiryDate, value);
    }

    private string _cvv = string.Empty;
    public string Cvv
    {
        get => _cvv;
        set => SetProperty(ref _cvv, value);
    }

    public ICommand ProcessPaymentCommand { get; }
    public ICommand GoBackCommand { get; }

    public CheckoutViewModel(CartService cartService, ApiService apiService)
    {
        _cartService = cartService;
        _apiService = apiService;
        Title = "Checkout";

        ProcessPaymentCommand = new Command(OnProcessPayment);
        GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        
        Task.Run(InitializePaymentAsync);
    }

    private async Task InitializePaymentAsync()
    {
        IsBusy = true;
        var basket = await _apiService.CreateOrUpdatePaymentIntentAsync(CartService.BasketId);
        if (basket != null)
        {
            PaymentIntentId = basket.PaymentIntentId ?? string.Empty;
            ClientSecret = basket.ClientSecret ?? string.Empty;
        }
        IsBusy = false;
    }

    private async void OnProcessPayment()
    {
        if (string.IsNullOrEmpty(CardNumber) || string.IsNullOrEmpty(ExpiryDate) || string.IsNullOrEmpty(Cvv))
        {
            if (App.Current?.MainPage != null)
                await App.Current.MainPage.DisplayAlert("Error", "Please fill in all payment details.", "OK");
            return;
        }

        IsBusy = true;
        
        // Mock Stripe payment process using ClientSecret...
        await Task.Delay(2000); 

        IsBusy = false;

        if (App.Current?.MainPage != null)
            await App.Current.MainPage.DisplayAlert("Payment Successful! 🎉", "Your order has been placed securely.", "OK");

        await _cartService.ClearCartAsync();
        await Shell.Current.GoToAsync("//HomePage");
    }
}
