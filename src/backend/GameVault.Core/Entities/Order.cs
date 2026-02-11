using GameVault.Core.Common;
using GameVault.Core.Enums;

namespace GameVault.Core.Entities;

public class Order : BaseEntity
{
    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? ShippingAddress { get; set; } // JSON blob for mockup
    public string? Notes { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
