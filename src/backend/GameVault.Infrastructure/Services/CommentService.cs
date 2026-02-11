using GameVault.Core.DTOs.Comments;
using GameVault.Core.DTOs.Common;
using GameVault.Core.Entities;
using GameVault.Core.Interfaces;
using GameVault.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public CommentService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<PagedResult<CommentDto>> GetCommentsByProductIdAsync(Guid productId, int pageNumber, int pageSize)
    {
        var query = _context.Comments
            .Include(c => c.User)
            .Where(c => c.ProductId == productId && c.IsApproved)
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                UserId = c.UserId,
                UserName = $"{c.User.FirstName} {c.User.LastName}",
                Title = c.Title,
                Body = c.Body,
                Rating = c.Rating,
                IsApproved = c.IsApproved,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<CommentDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<CommentDto?> GetCommentByIdAsync(Guid id)
    {
        var comment = await _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment == null) return null;

        return new CommentDto
        {
            Id = comment.Id,
            ProductId = comment.ProductId,
            UserId = comment.UserId,
            UserName = $"{comment.User.FirstName} {comment.User.LastName}",
            Title = comment.Title,
            Body = comment.Body,
            Rating = comment.Rating,
            IsApproved = comment.IsApproved,
            CreatedAt = comment.CreatedAt
        };
    }

    public async Task<CommentDto> CreateCommentAsync(Guid userId, CreateCommentRequest request)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
        if (product == null)
        {
            throw new InvalidOperationException("Product not found");
        }

        var comment = new Comment
        {
            ProductId = request.ProductId,
            UserId = userId,
            Title = request.Title,
            Body = request.Body,
            Rating = request.Rating,
            IsApproved = false // Comments require approval
        };

        await _unitOfWork.Comments.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        // Update product rating
        await UpdateProductRatingAsync(request.ProductId);

        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        return new CommentDto
        {
            Id = comment.Id,
            ProductId = comment.ProductId,
            UserId = comment.UserId,
            UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
            Title = comment.Title,
            Body = comment.Body,
            Rating = comment.Rating,
            IsApproved = comment.IsApproved,
            CreatedAt = comment.CreatedAt
        };
    }

    public async Task<CommentDto?> UpdateCommentAsync(Guid id, Guid userId, UpdateCommentRequest request)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(id);
        if (comment == null) return null;

        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only edit your own comments");
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
            comment.Title = request.Title;

        if (!string.IsNullOrWhiteSpace(request.Body))
            comment.Body = request.Body;

        if (request.Rating.HasValue)
            comment.Rating = request.Rating.Value;

        await _unitOfWork.Comments.UpdateAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        // Update product rating if rating changed
        if (request.Rating.HasValue)
        {
            await UpdateProductRatingAsync(comment.ProductId);
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        return new CommentDto
        {
            Id = comment.Id,
            ProductId = comment.ProductId,
            UserId = comment.UserId,
            UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
            Title = comment.Title,
            Body = comment.Body,
            Rating = comment.Rating,
            IsApproved = comment.IsApproved,
            CreatedAt = comment.CreatedAt
        };
    }

    public async Task<bool> DeleteCommentAsync(Guid id, Guid userId, bool isAdmin)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(id);
        if (comment == null) return false;

        if (!isAdmin && comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete your own comments");
        }

        var productId = comment.ProductId;

        await _unitOfWork.Comments.DeleteAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        // Update product rating after deletion
        await UpdateProductRatingAsync(productId);

        return true;
    }

    public async Task<bool> ApproveCommentAsync(Guid id)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(id);
        if (comment == null) return false;

        comment.IsApproved = true;

        await _unitOfWork.Comments.UpdateAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private async Task UpdateProductRatingAsync(Guid productId)
    {
        var approvedComments = await _unitOfWork.Comments.FindAsync(c => c.ProductId == productId && c.IsApproved);
        var commentsList = approvedComments.ToList();

        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        if (product == null) return;

        if (commentsList.Any())
        {
            product.Rating = (decimal)commentsList.Average(c => c.Rating);
            product.ReviewCount = commentsList.Count;
        }
        else
        {
            product.Rating = 0;
            product.ReviewCount = 0;
        }

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }
}
