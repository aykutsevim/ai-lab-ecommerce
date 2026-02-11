using GameVault.Core.DTOs.Common;
using GameVault.Core.DTOs.Orders;
using GameVault.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<OrderDto>>>> GetAllOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var orders = await _orderService.GetOrdersAsync(pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<OrderDto>>.SuccessResponse(orders));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PagedResult<OrderDto>>.ErrorResponse("An error occurred while retrieving orders", new List<string> { ex.Message }));
        }
    }

    [HttpGet("my-orders")]
    public async Task<ActionResult<ApiResponse<PagedResult<OrderDto>>>> GetMyOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<PagedResult<OrderDto>>.ErrorResponse("Invalid user"));
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<OrderDto>>.SuccessResponse(orders));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PagedResult<OrderDto>>.ErrorResponse("An error occurred while retrieving orders", new List<string> { ex.Message }));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrderById(Guid id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(ApiResponse<OrderDto>.ErrorResponse("Order not found"));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId) || order.UserId != userId))
            {
                return Forbid();
            }

            return Ok(ApiResponse<OrderDto>.SuccessResponse(order));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResponse("An error occurred while retrieving the order", new List<string> { ex.Message }));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<OrderDto>.ErrorResponse("Invalid user"));
            }

            var order = await _orderService.CreateOrderAsync(userId, request);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, ApiResponse<OrderDto>.SuccessResponse(order, "Order created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResponse("An error occurred while creating the order", new List<string> { ex.Message }));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var order = await _orderService.UpdateOrderStatusAsync(id, request.Status);
            if (order == null)
            {
                return NotFound(ApiResponse<OrderDto>.ErrorResponse("Order not found"));
            }

            return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order status updated successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResponse("An error occurred while updating the order status", new List<string> { ex.Message }));
        }
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<bool>>> CancelOrder(Guid id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("Invalid user"));
            }

            var result = await _orderService.CancelOrderAsync(id, userId);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Order not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Order cancelled successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while cancelling the order", new List<string> { ex.Message }));
        }
    }
}
