namespace GameVault.Core.DTOs.Products;

public class ProductDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Brand { get; set; }
    public required string ShortDescription { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public required string Currency { get; set; }
    public bool InStock { get; set; }
    public int StockCount { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public string? Subcategory { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<ProductVariantDto> Variants { get; set; } = new();
    public List<ProductImageDto> Images { get; set; } = new();
    public List<ProductSpecDto> Specs { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}
