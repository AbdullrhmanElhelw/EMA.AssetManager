using EMA.AssetManager.Domain.Data;
using EMA.AssetManager.Domain.Enums;
using EMA.AssetManager.Services.Dtos.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace EMA.AssetManager.Services.Services;

public class DashboardService : IDashboardService
{
    private readonly AssertManagerDbContext _dbContext;

    public DashboardService(AssertManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        var dto = new DashboardDto();

        // 1. الأعداد الكلية (واحدة تلو الأخرى)
        dto.TotalCategories = await _dbContext.Categories.CountAsync(cancellationToken);
        dto.TotalItems = await _dbContext.Items.CountAsync(cancellationToken);
        dto.TotalWarehouses = await _dbContext.Warehouses.CountAsync(cancellationToken);
        dto.TotalAssets = await _dbContext.Assets.CountAsync(cancellationToken);

        // 2. تفاصيل حالة العهدة
        dto.AssetsAvailable = await _dbContext.Assets.CountAsync(a => a.Status == AssetStatus.Available, cancellationToken);
        dto.AssetsInUse = await _dbContext.Assets.CountAsync(a => a.Status == AssetStatus.InUse, cancellationToken);
        dto.AssetsUnderMaintenance = await _dbContext.Assets.CountAsync(a => a.Status == AssetStatus.UnderMaintenance, cancellationToken);
        dto.AssetsDamaged = await _dbContext.Assets.CountAsync(a => a.Status == AssetStatus.Damaged || a.Status == AssetStatus.Retired, cancellationToken);

        // 3. النواقص
        dto.LowStockItemsCount = await _dbContext.Items.CountAsync(i => i.Quantity < 5, cancellationToken);

        return dto;
    }
}