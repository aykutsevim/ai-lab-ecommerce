using GameVault.Core.Common;

namespace GameVault.Core.Entities;

public class Product : BaseEntity
{
    public Guid CategoryId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Brand { get; set; }
    public required string ShortDescription { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public bool InStock { get; set; } = true;
    public int StockCount { get; set; } = 0;
    public decimal Rating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    public string? Subcategory { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Category Category { get; set; } = null!;
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductSpec> Specs { get; set; } = new List<ProductSpec>();
    public ICollection<ProductTag> Tags { get; set; } = new List<ProductTag>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
