using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using E_Commerce.Models;
using E_Commerce.Models.Api;

namespace E_Commerce.Services;

public class CartService
{
    private readonly ApiService _apiService;
    public const string BasketId = "shared_basket_1";

    public ObservableCollection<CartItem> CartItems { get; } = new();

    public CartService(ApiService apiService)
    {
        _apiService = apiService;
        Task.Run(InitializeCartAsync);
    }

    public async Task InitializeCartAsync()
    {
        var basket = await _apiService.GetBasketAsync(BasketId);
        if (basket != null && basket.Items != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CartItems.Clear();
                foreach (var item in basket.Items)
                {
                    CartItems.Add(new CartItem
                    {
                        Product = new Product 
                        { 
                            Id = item.Id, 
                            Name = item.ProductName, 
                            Price = item.Price, 
                            Image = string.IsNullOrEmpty(item.PictureUrl) || item.PictureUrl.StartsWith("http") 
                                    ? item.PictureUrl 
                                    : $"https://mauiapp.runasp.net/{item.PictureUrl.TrimStart('/')}", 
                            Category = item.Category 
                        },
                        Quantity = item.Quantity
                    });
                }
            });
        }
    }

    public async Task AddProductAsync(Product product, int quantity)
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
        await SyncWithBackendAsync();
    }

    public async Task RemoveItemAsync(CartItem item)
    {
        CartItems.Remove(item);
        await SyncWithBackendAsync();
    }

    public async Task UpdateQuantityAsync(CartItem item, int quantity)
    {
        item.Quantity = quantity;
        await SyncWithBackendAsync();
    }

    public async Task ClearCartAsync()
    {
        CartItems.Clear();
        await _apiService.DeleteBasketAsync(BasketId);
    }

    private async Task SyncWithBackendAsync()
    {
        var basket = new CustomerBasketDto
        {
            Id = BasketId,
            DeliveryMethodId = 3,
            Items = CartItems.Select(c => new BasketItemDto
            {
                Id = c.Product.Id,
                ProductName = c.Product.Name,
                Price = c.Product.Price,
                Quantity = c.Quantity,
                PictureUrl = c.Product.Image,
                Brand = "N/A",
                Category = c.Product.Category
            }).ToList()
        };
        await _apiService.UpdateBasketAsync(basket);
    }
}
