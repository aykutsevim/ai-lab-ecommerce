using GameVault.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameVault.Infrastructure.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pv => pv.Color)
            .HasMaxLength(50);

        builder.Property(pv => pv.ImageUrl)
            .HasMaxLength(500);

        builder.Property(pv => pv.AdditionalPrice)
            .HasColumnType("decimal(10,2)");

        builder.HasIndex(pv => pv.ProductId);
        builder.HasIndex(pv => new { pv.ProductId, pv.Name });
    }
}
