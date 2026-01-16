using EMA.AssetManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EMA.AssetManager.Domain.Data.Configurations;

internal sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Location)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.KeeperName)
            .HasMaxLength(100)
            .IsRequired(false);
    }
}

