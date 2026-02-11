namespace GameVault.Core.DTOs.Comments;

public class UpdateCommentRequest
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public int? Rating { get; set; }
}
