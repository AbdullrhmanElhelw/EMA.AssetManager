using EMA.AssetManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EMA.AssetManager.Domain.Data.Configurations;

internal sealed class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Unit)
            .IsRequired()
            .HasMaxLength(50);
    }
}
