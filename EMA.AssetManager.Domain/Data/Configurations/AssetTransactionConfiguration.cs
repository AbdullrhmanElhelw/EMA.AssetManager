using EMA.AssetManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EMA.AssetManager.Domain.Data.Configurations;

internal sealed class AssetTransactionConfiguration : IEntityTypeConfiguration<AssetTransaction>
{
    public void Configure(EntityTypeBuilder<AssetTransaction> builder)
    {
        builder.HasOne(t => t.Asset)
              .WithMany(a => a.AssetTransactions) // الأصل ممكن يكون له حركات كتير
              .HasForeignKey(t => t.AssetId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}

