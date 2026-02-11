namespace GameVault.Core.DTOs.Products;

public class ProductVariantDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public string? ImageUrl { get; set; }
    public decimal AdditionalPrice { get; set; }
    public int SortOrder { get; set; }
}
