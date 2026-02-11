namespace GameVault.Core.DTOs.Products;

public class ProductListDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Brand { get; set; }
    public required string ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public required string Currency { get; set; }
    public bool InStock { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public string? Subcategory { get; set; }
    public string? ThumbnailUrl { get; set; }
    public List<string> Tags { get; set; } = new();
}
