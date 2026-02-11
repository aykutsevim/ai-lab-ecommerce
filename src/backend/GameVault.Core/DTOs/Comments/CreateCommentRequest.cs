namespace GameVault.Core.DTOs.Comments;

public class CreateCommentRequest
{
    public Guid ProductId { get; set; }
    public string? Title { get; set; }
    public required string Body { get; set; }
    public int Rating { get; set; }
}
