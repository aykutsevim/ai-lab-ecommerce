using GameVault.Core.Common;

namespace GameVault.Core.Entities;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }
    public required string Url { get; set; }
    public required string Alt { get; set; }
    public string? VariantName { get; set; }
    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public Product Product { get; set; } = null!;
}
