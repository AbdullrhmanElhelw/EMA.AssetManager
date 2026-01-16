using EMA.AssetManager.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EMA.AssetManager.Domain.Data;

public class AssertManagerDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AssertManagerDbContext(DbContextOptions<AssertManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Item> Items => Set<Item>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<AssetTransaction> AssetTransactions => Set<AssetTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(b => { b.ToTable("Users"); });
        modelBuilder.Entity<IdentityRole<Guid>>(b => { b.ToTable("Roles"); });
        modelBuilder.Entity<IdentityUserRole<Guid>>(b => { b.ToTable("UserRoles"); });
        modelBuilder.Entity<IdentityUserClaim<Guid>>(b => { b.ToTable("UserClaims"); });
        modelBuilder.Entity<IdentityUserLogin<Guid>>(b => { b.ToTable("UserLogins"); });
        modelBuilder.Entity<IdentityRoleClaim<Guid>>(b => { b.ToTable("RoleClaims"); });
        modelBuilder.Entity<IdentityUserToken<Guid>>(b => { b.ToTable("UserTokens"); });


        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssertManagerDbContext).Assembly);
    }
}
