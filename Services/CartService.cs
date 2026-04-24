using System.Collections.ObjectModel;
using System.Linq;
using E_Commerce.Models;

namespace E_Commerce.Services;

public static class CartService
{
    public static ObservableCollection<CartItem> CartItems { get; } = new();

    public static void AddProduct(Product product, int quantity)
    {
        var existing = CartItems.FirstOrDefault(x => x.Product.Id == product.Id);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            CartItems.Add(new CartItem { Product = product, Quantity = quantity });
        }
    }

    public static void RemoveItem(CartItem item)
    {
        CartItems.Remove(item);
    }

    public static void ClearCart()
    {
        CartItems.Clear();
    }
}
