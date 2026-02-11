namespace GameVault.Core.DTOs.Orders;

public class CreateOrderRequest
{
    public required List<CreateOrderItemRequest> Items { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Notes { get; set; }
}
