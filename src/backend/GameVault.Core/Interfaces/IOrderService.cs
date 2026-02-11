using GameVault.Core.DTOs.Common;
using GameVault.Core.DTOs.Orders;
using GameVault.Core.Enums;

namespace GameVault.Core.Interfaces;

public interface IOrderService
{
    Task<PagedResult<OrderDto>> GetOrdersAsync(int pageNumber, int pageSize);
    Task<PagedResult<OrderDto>> GetOrdersByUserIdAsync(Guid userId, int pageNumber, int pageSize);
    Task<OrderDto?> GetOrderByIdAsync(Guid id);
    Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderRequest request);
    Task<OrderDto?> UpdateOrderStatusAsync(Guid id, OrderStatus status);
    Task<bool> CancelOrderAsync(Guid id, Guid userId);
}
