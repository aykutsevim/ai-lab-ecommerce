using GameVault.Core.DTOs.Comments;
using GameVault.Core.DTOs.Common;
using GameVault.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<ApiResponse<PagedResult<CommentDto>>>> GetCommentsByProduct(Guid productId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var comments = await _commentService.GetCommentsByProductIdAsync(productId, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<CommentDto>>.SuccessResponse(comments));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PagedResult<CommentDto>>.ErrorResponse("An error occurred while retrieving comments", new List<string> { ex.Message }));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CommentDto>>> GetCommentById(Guid id)
    {
        try
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound(ApiResponse<CommentDto>.ErrorResponse("Comment not found"));
            }

            return Ok(ApiResponse<CommentDto>.SuccessResponse(comment));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CommentDto>.ErrorResponse("An error occurred while retrieving the comment", new List<string> { ex.Message }));
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CommentDto>>> CreateComment([FromBody] CreateCommentRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<CommentDto>.ErrorResponse("Invalid user"));
            }

            var comment = await _commentService.CreateCommentAsync(userId, request);
            return CreatedAtAction(nameof(GetCommentById), new { id = comment.Id }, ApiResponse<CommentDto>.SuccessResponse(comment, "Comment created successfully. It will be visible after approval."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CommentDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CommentDto>.ErrorResponse("An error occurred while creating the comment", new List<string> { ex.Message }));
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CommentDto>>> UpdateComment(Guid id, [FromBody] UpdateCommentRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<CommentDto>.ErrorResponse("Invalid user"));
            }

            var comment = await _commentService.UpdateCommentAsync(id, userId, request);
            if (comment == null)
            {
                return NotFound(ApiResponse<CommentDto>.ErrorResponse("Comment not found"));
            }

            return Ok(ApiResponse<CommentDto>.SuccessResponse(comment, "Comment updated successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CommentDto>.ErrorResponse("An error occurred while updating the comment", new List<string> { ex.Message }));
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteComment(Guid id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("Invalid user"));
            }

            var isAdmin = User.IsInRole("Admin");
            var result = await _commentService.DeleteCommentAsync(id, userId, isAdmin);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Comment not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Comment deleted successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting the comment", new List<string> { ex.Message }));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<ApiResponse<bool>>> ApproveComment(Guid id)
    {
        try
        {
            var result = await _commentService.ApproveCommentAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Comment not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Comment approved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while approving the comment", new List<string> { ex.Message }));
        }
    }
}
