using EMA.AssetManager.Domain.Data;
using EMA.AssetManager.Domain.Entities;
using EMA.AssetManager.Services.Dtos.Warehouses;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EMA.AssetManager.Services.Interfaces;

public class WarehouseService : IWarehouseService
{
    private readonly AssertManagerDbContext _dbContext;

    public WarehouseService(AssertManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<WarehouseDto>> GetWarehousesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Warehouses
            .AsNoTracking()
            .OrderBy(w => w.Name)
            .Select(w => new WarehouseDto
            {
                Id = w.Id,
                Name = w.Name,
                Location = w.Location,
                KeeperName = w.KeeperName,
                IsActive = w.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<WarehouseDto?> GetWarehouseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var warehouse = await _dbContext.Warehouses.FindAsync(new object[] { id }, cancellationToken);
        if (warehouse == null) return null;

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Location = warehouse.Location,
            KeeperName = warehouse.KeeperName,
            IsActive = warehouse.IsActive
        };
    }

    public async Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto dto, CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Warehouses.AnyAsync(w => w.Name == dto.Name, cancellationToken))
            throw new Exception($"المخزن '{dto.Name}' موجود بالفعل.");

        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Location = dto.Location,
            KeeperName = dto.KeeperName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Warehouses.Add(warehouse);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetWarehouseByIdAsync(warehouse.Id, cancellationToken)!;
    }

    public async Task<WarehouseDto> UpdateWarehouseAsync(Guid id, UpdateWarehouseDto dto, CancellationToken cancellationToken = default)
    {
        var warehouse = await _dbContext.Warehouses.FindAsync(new object[] { id }, cancellationToken);
        if (warehouse == null) throw new Exception("المخزن غير موجود");

        if (warehouse.Name != dto.Name && await _dbContext.Warehouses.AnyAsync(w => w.Name == dto.Name, cancellationToken))
            throw new Exception($"المخزن '{dto.Name}' موجود بالفعل.");

        warehouse.Name = dto.Name;
        warehouse.Location = dto.Location;
        warehouse.KeeperName = dto.KeeperName;
        warehouse.IsActive = dto.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetWarehouseByIdAsync(id, cancellationToken)!;
    }

    public async Task DeleteWarehouseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var warehouse = await _dbContext.Warehouses.FindAsync(new object[] { id }, cancellationToken);
        if (warehouse == null) throw new Exception("المخزن غير موجود");

        // هنا مستقبلاً ممكن نضيف شرط: ممنوع الحذف لو فيه عهدة

        _dbContext.Warehouses.Remove(warehouse);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}