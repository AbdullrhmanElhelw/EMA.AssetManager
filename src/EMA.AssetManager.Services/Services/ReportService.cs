using EMA.AssetManager.Domain.Data;
using EMA.AssetManager.Domain.Enums;
using EMA.AssetManager.Services.Dtos.Reports;
using Microsoft.EntityFrameworkCore;

public class ReportService : IReportService
{
    private readonly AssertManagerDbContext _context;

    public ReportService(AssertManagerDbContext context)
    {
        _context = context;
    }

    public async Task<List<InventoryReportDto>> GetInventorySummaryAsync()
    {
        // هنجيب كل الأصناف ونعمل Grouping بالفئة (Category)
        var data = await _context.Categories
            .Include(c => c.Items)
            .ThenInclude(i => i.Assets)
            .Select(c => new InventoryReportDto
            {
                CategoryName = c.Name,
                TotalItemsCount = c.Items.Count,
                TotalAssetsCount = c.Items.SelectMany(i => i.Assets).Count(),

                // حساب الحالات
                AvailableCount = c.Items.SelectMany(i => i.Assets).Count(a => a.Status == AssetStatus.Available),
                AssignedCount = c.Items.SelectMany(i => i.Assets).Count(a => a.Status == AssetStatus.InUse),
                MaintenanceCount = c.Items.SelectMany(i => i.Assets).Count(a => a.Status == AssetStatus.UnderMaintenance),
                DamagedCount = c.Items.SelectMany(i => i.Assets).Count(a => a.Status == AssetStatus.Damaged || a.Status == AssetStatus.Retired)
            })
            .ToListAsync();

        return data;
    }
}