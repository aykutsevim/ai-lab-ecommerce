using GameVault.Core.Common;

namespace GameVault.Core.Entities;

public class ProductTag : BaseEntity
{
    public Guid ProductId { get; set; }
    public required string Tag { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
}
