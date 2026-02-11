using GameVault.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameVault.Infrastructure.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasMaxLength(200);

        builder.Property(c => c.Body)
            .IsRequired();

        builder.Property(c => c.Rating)
            .IsRequired();

        builder.HasIndex(c => c.ProductId);
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.Rating);
        builder.HasIndex(c => c.CreatedAt);

        // Check constraint for rating
        builder.ToTable(t => t.HasCheckConstraint("CK_Comment_Rating", "[Rating] >= 1 AND [Rating] <= 5"));
    }
}
