using E_Commerce.ViewModels;

namespace E_Commerce.Models;

public class CartItem : BaseViewModel
{
    private Product _product = new();
    public Product Product
    {
        get => _product;
        set => SetProperty(ref _product, value);
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (SetProperty(ref _quantity, value))
            {
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
    }

    public decimal TotalPrice => Product.Price * Quantity;
}
