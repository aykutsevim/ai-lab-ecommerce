using GameVault.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameVault.Infrastructure.Configurations;

public class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
{
    public void Configure(EntityTypeBuilder<ProductTag> builder)
    {
        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.Tag)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(pt => new { pt.ProductId, pt.Tag })
            .IsUnique();

        builder.HasIndex(pt => pt.Tag);
    }
}
