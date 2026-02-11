using GameVault.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameVault.Infrastructure.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pi => pi.Alt)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pi => pi.VariantName)
            .HasMaxLength(100);

        builder.HasIndex(pi => pi.ProductId);
        builder.HasIndex(pi => new { pi.ProductId, pi.SortOrder });
    }
}
