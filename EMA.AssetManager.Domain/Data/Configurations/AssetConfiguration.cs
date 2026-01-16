using EMA.AssetManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EMA.AssetManager.Domain.Data.Configurations;

internal sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasIndex(a => a.SerialNumber).IsUnique();
        builder.HasIndex(a => a.Barcode).IsUnique();
        builder.Property(a => a.SerialNumber).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Barcode).IsRequired().HasMaxLength(100);

        builder.HasOne(a => a.Item)
              .WithMany(i => i.Assets)
              .HasForeignKey(a => a.ItemId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Warehouse)
              .WithMany(w => w.Assets)
              .HasForeignKey(a => a.WarehouseId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}

