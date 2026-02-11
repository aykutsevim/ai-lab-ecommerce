namespace GameVault.Core.DTOs.Comments;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public required string UserName { get; set; }
    public string? Title { get; set; }
    public required string Body { get; set; }
    public int Rating { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}
