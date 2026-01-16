using EMA.AssetManager.Domain.Data;
using EMA.AssetManager.Domain.Entities;
using EMA.AssetManager.Domain.Enums;
using EMA.AssetManager.Services.Dtos.Assets;
using Microsoft.EntityFrameworkCore;

public class AssetService : IAssetService
{
    private readonly AssertManagerDbContext _dbContext;

    public AssetService(AssertManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<AssetDto>> GetAssetsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Assets
            .Include(a => a.Item)       // عشان نجيب اسم الصنف
            .Include(a => a.Warehouse)  // عشان نجيب اسم المخزن
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AssetDto
            {
                Id = a.Id,
                SerialNumber = a.SerialNumber,
                Barcode = a.Barcode,
                Status = a.Status,
                PurchaseDate = a.PurchaseDate,
                ExpiryDate = a.ExpiryDate,
                ItemId = a.ItemId,
                ItemName = a.Item.Name,
                ItemCode = a.Item.Code,
                WarehouseId = a.WarehouseId,
                WarehouseName = a.Warehouse.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AssetDto?> GetAssetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var asset = await _dbContext.Assets
            .Include(a => a.Item)
            .Include(a => a.Warehouse)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (asset == null) return null;

        return new AssetDto
        {
            Id = asset.Id,
            SerialNumber = asset.SerialNumber,
            Barcode = asset.Barcode,
            Status = asset.Status,
            PurchaseDate = asset.PurchaseDate,
            ExpiryDate = asset.ExpiryDate,
            ItemId = asset.ItemId,
            ItemName = asset.Item.Name,
            ItemCode = asset.Item.Code,
            WarehouseId = asset.WarehouseId,
            WarehouseName = asset.Warehouse.Name
        };
    }

    public async Task<AssetDto> CreateAssetAsync(CreateAssetDto dto, CancellationToken cancellationToken = default)
    {
        // 1. التحقق من وجود الصنف والمخزن
        if (!await _dbContext.Items.AnyAsync(i => i.Id == dto.ItemId, cancellationToken))
            throw new Exception("الصنف المختار غير موجود.");

        if (!await _dbContext.Warehouses.AnyAsync(w => w.Id == dto.WarehouseId, cancellationToken))
            throw new Exception("المخزن المختار غير موجود.");

        // 2. التحقق من عدم تكرار السيريال أو الباركود
        if (await _dbContext.Assets.AnyAsync(a => a.SerialNumber == dto.SerialNumber, cancellationToken))
            throw new Exception($"السيريال '{dto.SerialNumber}' موجود بالفعل لقطعة أخرى.");

        if (!string.IsNullOrEmpty(dto.Barcode) && await _dbContext.Assets.AnyAsync(a => a.Barcode == dto.Barcode, cancellationToken))
            throw new Exception($"الباركود '{dto.Barcode}' موجود بالفعل.");

        var asset = new Asset
        {
            Id = Guid.NewGuid(),
            ItemId = dto.ItemId,
            WarehouseId = dto.WarehouseId,
            SerialNumber = dto.SerialNumber,
            Barcode = dto.Barcode,
            Status = dto.Status,
            PurchaseDate = dto.PurchaseDate,
            ExpiryDate = dto.ExpiryDate,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Assets.Add(asset);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetAssetByIdAsync(asset.Id, cancellationToken)!;
    }

    public async Task<AssetDto> UpdateAssetAsync(Guid id, UpdateAssetDto dto, CancellationToken cancellationToken = default)
    {
        var asset = await _dbContext.Assets.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (asset == null) throw new Exception("القطعة غير موجودة.");

        // التحقق من تكرار السيريال (لو اتغير)
        if (asset.SerialNumber != dto.SerialNumber && await _dbContext.Assets.AnyAsync(a => a.SerialNumber == dto.SerialNumber, cancellationToken))
            throw new Exception($"السيريال '{dto.SerialNumber}' مستخدم بالفعل.");

        asset.SerialNumber = dto.SerialNumber;
        asset.Barcode = dto.Barcode;
        asset.Status = dto.Status;
        asset.PurchaseDate = dto.PurchaseDate;
        asset.ExpiryDate = dto.ExpiryDate;
        asset.WarehouseId = dto.WarehouseId; // تحديث المكان

        await _dbContext.SaveChangesAsync(cancellationToken);
        return await GetAssetByIdAsync(id, cancellationToken)!;
    }

    public async Task DeleteAssetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var asset = await _dbContext.Assets.FindAsync(new object[] { id }, cancellationToken);
        if (asset == null) throw new Exception("القطعة غير موجودة.");

        _dbContext.Assets.Remove(asset);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AssetDto?> GetAssetByIdAsync(Guid id)
    {
        var asset = await _dbContext.Assets
            .Include(a => a.Item)
            .Include(a => a.Warehouse)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (asset == null)
            return null;

        return new AssetDto
        {
            Id = asset.Id,
            SerialNumber = asset.SerialNumber,
            ItemName = asset.Item?.Name ?? "",
            WarehouseName = asset.Warehouse?.Name ?? "",
            Status = asset.Status,
            // ... باقي الخصائص
        };
    }
    public async Task UpdateAssetStatusAsync(Guid assetId, AssetStatus newStatus)
    {
        var asset = await _dbContext.Assets.FindAsync(assetId);
        if (asset != null)
        {
            asset.Status = newStatus;
            await _dbContext.SaveChangesAsync();
        }
    }

}
