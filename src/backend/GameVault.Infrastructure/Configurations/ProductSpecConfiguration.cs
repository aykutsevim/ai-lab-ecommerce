using GameVault.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameVault.Infrastructure.Configurations;

public class ProductSpecConfiguration : IEntityTypeConfiguration<ProductSpec>
{
    public void Configure(EntityTypeBuilder<ProductSpec> builder)
    {
        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ps => ps.Value)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(ps => ps.ProductId);
        builder.HasIndex(ps => new { ps.ProductId, ps.Key });
    }
}
