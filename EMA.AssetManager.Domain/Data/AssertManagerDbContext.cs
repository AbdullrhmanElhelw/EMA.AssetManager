using EMA.AssetManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMA.AssetManager.Domain.Data;

public class AssertManagerDbContext : DbContext
{
    public AssertManagerDbContext(DbContextOptions<AssertManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Item> Items => Set<Item>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<AssetTransaction> AssetTransactions => Set<AssetTransaction>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<MaintenanceTicket> MaintenanceTickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssertManagerDbContext).Assembly);
    }
}
