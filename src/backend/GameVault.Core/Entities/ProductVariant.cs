using GameVault.Core.Common;

namespace GameVault.Core.Entities;

public class ProductVariant : BaseEntity
{
    public Guid ProductId { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public string? ImageUrl { get; set; }
    public decimal AdditionalPrice { get; set; } = 0;
    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public Product Product { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
