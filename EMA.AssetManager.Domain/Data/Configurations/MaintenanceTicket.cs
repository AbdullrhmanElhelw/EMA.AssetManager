using EMA.AssetManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EMA.AssetManager.Domain.Data.Configurations;

internal sealed class MaintenanceTicketConfiguration : IEntityTypeConfiguration<MaintenanceTicket>
{
    public void Configure(EntityTypeBuilder<MaintenanceTicket> builder)
    {
        builder.HasKey(mt => mt.Id);
        builder.Property(mt => mt.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(mt => mt.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(mt => mt.ReportedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mt => mt.Priority)
            .IsRequired();

        builder.Property(mt => mt.Status)
            .IsRequired();

        builder.Property(mt => mt.Cost)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.HasOne(mt => mt.Asset)
            .WithMany(a => a.MaintenanceTickets)
            .HasForeignKey(mt => mt.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
