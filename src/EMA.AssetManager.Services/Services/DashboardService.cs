using EMA.AssetManager.Domain.Data;
using EMA.AssetManager.Domain.Enums;
using EMA.AssetManager.Services.Dtos.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace EMA.AssetManager.Services.Services;

public class DashboardService : IDashboardService
{
    private readonly IDbContextFactory<AssertManagerDbContext> _contextFactory;

    // بنستخدم Factory عشان نضمن خيوط معالجة منفصلة لو حبينا نسرع أكتر
    public DashboardService(IDbContextFactory<AssertManagerDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<DashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var dto = new DashboardDto();

        // 1. الأعداد الكلية (سريعة جداً)
        dto.TotalCategories = await context.Categories.CountAsync(cancellationToken);
        dto.TotalItems = await context.Items.CountAsync(cancellationToken);
        dto.TotalWarehouses = await context.Warehouses.CountAsync(cancellationToken);
        dto.TotalAssets = await context.Assets.CountAsync(cancellationToken);

        // 2. 🔥 التعديل الجوهري: جلب توزيع الحالات في استعلام واحد (Group By)
        // ده أسرع بكتير من إنك تعمل Count 4 مرات
        var statusDistribution = await context.Assets
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        dto.AssetsAvailable = statusDistribution.FirstOrDefault(x => x.Status == AssetStatus.Available)?.Count ?? 0;
        dto.AssetsInUse = statusDistribution.FirstOrDefault(x => x.Status == AssetStatus.InUse)?.Count ?? 0;
        dto.AssetsUnderMaintenance = statusDistribution.FirstOrDefault(x => x.Status == AssetStatus.UnderMaintenance)?.Count ?? 0;

        // تجميع التالف والكهنة مع بعض
        dto.AssetsDamaged = statusDistribution
            .Where(x => x.Status == AssetStatus.Damaged || x.Status == AssetStatus.Retired)
            .Sum(x => x.Count);

        // 3. النواقص (بناءً على الإعدادات)
        // بنجيب حد النواقص من جدول الإعدادات، لو مش موجود بنعتبره 5
        var settings = await context.SystemSettings.FirstOrDefaultAsync(cancellationToken);
        int lowStockThreshold = settings?.LowStockThreshold ?? 5;

        dto.LowStockItemsCount = await context.Items
            .CountAsync(i => i.Quantity < lowStockThreshold, cancellationToken);

        return dto;
    }
}