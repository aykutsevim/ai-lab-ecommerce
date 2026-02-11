using GameVault.Core.Enums;

namespace GameVault.Core.DTOs.Orders;

public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
}
