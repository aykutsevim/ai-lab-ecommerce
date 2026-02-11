using GameVault.Core.DTOs.Common;
using GameVault.Core.DTOs.Orders;
using GameVault.Core.Entities;
using GameVault.Core.Enums;
using GameVault.Core.Interfaces;
using GameVault.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public OrderService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<PagedResult<OrderDto>> GetOrdersAsync(int pageNumber, int pageSize)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Variant)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(o => MapToOrderDto(o))
            .ToListAsync();

        return new PagedResult<OrderDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<OrderDto>> GetOrdersByUserIdAsync(Guid userId, int pageNumber, int pageSize)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Variant)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(o => MapToOrderDto(o))
            .ToListAsync();

        return new PagedResult<OrderDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Variant)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order == null ? null : MapToOrderDto(order);
    }

    public async Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderRequest request)
    {
        if (!request.Items.Any())
        {
            throw new InvalidOperationException("Order must contain at least one item");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                Currency = "USD",
                ShippingAddress = request.ShippingAddress,
                Notes = request.Notes,
                TotalAmount = 0
            };

            await _unitOfWork.Orders.AddAsync(order);

            decimal totalAmount = 0;

            foreach (var itemRequest in request.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(itemRequest.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product {itemRequest.ProductId} not found");
                }

                if (!product.InStock || product.StockCount < itemRequest.Quantity)
                {
                    throw new InvalidOperationException($"Product {product.Name} is out of stock");
                }

                decimal unitPrice = product.Price;

                if (itemRequest.VariantId.HasValue)
                {
                    var variant = await _unitOfWork.ProductVariants.GetByIdAsync(itemRequest.VariantId.Value);
                    if (variant == null || variant.ProductId != product.Id)
                    {
                        throw new InvalidOperationException($"Invalid variant for product {product.Name}");
                    }
                    unitPrice += variant.AdditionalPrice;
                }

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    VariantId = itemRequest.VariantId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = unitPrice
                };

                await _unitOfWork.OrderItems.AddAsync(orderItem);

                // Update stock
                product.StockCount -= itemRequest.Quantity;
                if (product.StockCount <= 0)
                {
                    product.InStock = false;
                }
                await _unitOfWork.Products.UpdateAsync(product);

                totalAmount += unitPrice * itemRequest.Quantity;
            }

            order.TotalAmount = totalAmount;
            await _unitOfWork.Orders.UpdateAsync(order);

            await _unitOfWork.CommitTransactionAsync();

            // Reload order with all relations
            var createdOrder = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Variant)
                .FirstAsync(o => o.Id == order.Id);

            return MapToOrderDto(createdOrder);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<OrderDto?> UpdateOrderStatusAsync(Guid id, OrderStatus status)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        if (order == null) return null;

        order.Status = status;
        await _unitOfWork.Orders.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        // Reload with relations
        var updatedOrder = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Variant)
            .FirstAsync(o => o.Id == id);

        return MapToOrderDto(updatedOrder);
    }

    public async Task<bool> CancelOrderAsync(Guid id, Guid userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return false;

        if (order.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only cancel your own orders");
        }

        if (order.Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Only pending orders can be cancelled");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Restore stock
            foreach (var item in order.OrderItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockCount += item.Quantity;
                    product.InStock = true;
                    await _unitOfWork.Products.UpdateAsync(product);
                }
            }

            order.Status = OrderStatus.Cancelled;
            await _unitOfWork.Orders.UpdateAsync(order);

            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private static OrderDto MapToOrderDto(Order o)
    {
        return new OrderDto
        {
            Id = o.Id,
            UserId = o.UserId,
            UserName = $"{o.User.FirstName} {o.User.LastName}",
            Status = o.Status,
            TotalAmount = o.TotalAmount,
            Currency = o.Currency,
            ShippingAddress = o.ShippingAddress,
            Notes = o.Notes,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            Items = o.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product.Name,
                VariantId = oi.VariantId,
                VariantName = oi.Variant?.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }
}
