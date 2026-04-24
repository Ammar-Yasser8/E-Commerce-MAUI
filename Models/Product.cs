namespace E_Commerce.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal OriginalPrice { get; set; }
    public string Image { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;        // e.g. "NEW", "SALE", "HOT"
    public bool IsFavorite { get; set; }
    public bool HasDiscount => OriginalPrice > Price;
    public string DiscountPercent => HasDiscount
        ? $"-{(int)((1 - Price / OriginalPrice) * 100)}%"
        : string.Empty;
}
