using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using E_Commerce.Models.Api;

namespace E_Commerce.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ApiService()
    {
        var handler = new HttpClientHandler();
        
        // Bypass SSL validation for local development
        #if DEBUG
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        #endif

        _httpClient = new HttpClient(handler);
        
        // Use the hosted API URL
        _baseUrl = "https://mauiapp.runasp.net/api/";

        _httpClient.BaseAddress = new Uri(_baseUrl);
        
        // Load persisted token if available
        if (AuthService.CurrentUser != null && !string.IsNullOrEmpty(AuthService.CurrentUser.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthService.CurrentUser.Token);
        }
    }

    // Account
    public async Task<UserDto?> LoginAsync(LoginDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("Account/Login", dto);
        if (response.IsSuccessStatusCode)
        {
            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            if (user != null && !string.IsNullOrEmpty(user.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
            }
            return user;
        }
        return null;
    }

    public async Task<UserDto?> RegisterAsync(RegisterDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("Account/Register", dto);
        if (response.IsSuccessStatusCode)
        {
            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            if (user != null && !string.IsNullOrEmpty(user.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
            }
            return user;
        }
        return null;
    }

    public void Logout()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    // Products
    public async Task<ProductToReverseDtoPagination?> GetProductsAsync(int? categoryId = null, int? brandId = null, string search = null, int pageIndex = 1, int pageSize = 10)
    {
        var query = new List<string>
        {
            $"pageIndex={pageIndex}",
            $"pageSize={pageSize}"
        };

        if (categoryId.HasValue) query.Add($"CategoryId={categoryId}");
        if (brandId.HasValue) query.Add($"BrandId={brandId}");
        if (!string.IsNullOrEmpty(search)) query.Add($"Search={search}");

        var url = "Products";
        if (query.Any()) url += "?" + string.Join("&", query);

        return await _httpClient.GetFromJsonAsync<ProductToReverseDtoPagination>(url);
    }

    public async Task<List<ProductCategory>?> GetCategoriesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ProductCategory>>("Products/categories");
    }

    public async Task<List<ProductBrand>?> GetBrandsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ProductBrand>>("Products/brands");
    }

    // Basket
    public async Task<CustomerBasketDto?> GetBasketAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<CustomerBasketDto>($"Basket?id={id}");
    }

    public async Task<CustomerBasketDto?> UpdateBasketAsync(CustomerBasketDto basket)
    {
        var response = await _httpClient.PostAsJsonAsync("Basket", basket);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CustomerBasketDto>();
        }
        return null;
    }

    public async Task DeleteBasketAsync(string id)
    {
        await _httpClient.DeleteAsync($"Basket?id={id}");
    }

    // Payments
    public async Task<CustomerBasketDto?> CreateOrUpdatePaymentIntentAsync(string basketId)
    {
        var response = await _httpClient.PostAsync($"Payments/{basketId}", null);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CustomerBasketDto>();
        }
        return null;
    }

    // Admin Products
    public async Task<bool> CreateProductAsync(MultipartFormDataContent content)
    {
        var response = await _httpClient.PostAsync("Products", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateProductAsync(int id, MultipartFormDataContent content)
    {
        var response = await _httpClient.PutAsync($"Products/{id}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateCategoryAsync(MultipartFormDataContent content)
    {
        var response = await _httpClient.PostAsync("Products/categories", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCategoryAsync(int id, MultipartFormDataContent content)
    {
        var response = await _httpClient.PutAsync($"Products/categories/{id}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"Products/{id}");
        return response.IsSuccessStatusCode;
    }

    // Admin Orders
    public async Task<List<OrderDto>?> GetAllOrdersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<OrderDto>>("Orders/all");
    }

    public async Task<bool> UpdateOrderStatusAsync(int id, string status)
    {
        var response = await _httpClient.PutAsJsonAsync($"Orders/{id}/status", status);
        return response.IsSuccessStatusCode;
    }
}
