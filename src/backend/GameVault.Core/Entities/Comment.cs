using GameVault.Core.Common;

namespace GameVault.Core.Entities;

public class Comment : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public required string Body { get; set; }
    public int Rating { get; set; } // 1-5 stars
    public bool IsApproved { get; set; } = true;

    // Navigation properties
    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
}
