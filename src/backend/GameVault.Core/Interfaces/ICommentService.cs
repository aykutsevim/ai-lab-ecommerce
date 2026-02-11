using GameVault.Core.DTOs.Comments;
using GameVault.Core.DTOs.Common;

namespace GameVault.Core.Interfaces;

public interface ICommentService
{
    Task<PagedResult<CommentDto>> GetCommentsByProductIdAsync(Guid productId, int pageNumber, int pageSize);
    Task<CommentDto?> GetCommentByIdAsync(Guid id);
    Task<CommentDto> CreateCommentAsync(Guid userId, CreateCommentRequest request);
    Task<CommentDto?> UpdateCommentAsync(Guid id, Guid userId, UpdateCommentRequest request);
    Task<bool> DeleteCommentAsync(Guid id, Guid userId, bool isAdmin);
    Task<bool> ApproveCommentAsync(Guid id);
}
