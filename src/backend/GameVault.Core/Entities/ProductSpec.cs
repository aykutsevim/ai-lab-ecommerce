using GameVault.Core.Common;

namespace GameVault.Core.Entities;

public class ProductSpec : BaseEntity
{
    public Guid ProductId { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }
    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public Product Product { get; set; } = null!;
}
