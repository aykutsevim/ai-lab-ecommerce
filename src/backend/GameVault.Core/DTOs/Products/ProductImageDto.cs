namespace GameVault.Core.DTOs.Products;

public class ProductImageDto
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public required string Alt { get; set; }
    public string? VariantName { get; set; }
    public int SortOrder { get; set; }
}
