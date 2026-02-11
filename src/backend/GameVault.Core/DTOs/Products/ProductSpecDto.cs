namespace GameVault.Core.DTOs.Products;

public class ProductSpecDto
{
    public Guid Id { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }
    public int SortOrder { get; set; }
}
