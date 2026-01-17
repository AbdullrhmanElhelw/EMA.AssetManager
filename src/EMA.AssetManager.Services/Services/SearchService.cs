using EMA.AssetManager.Domain.Data;
using EMA.AssetManager.Services.Dtos.Search;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

public class SearchService : ISearchService
{
    private readonly IDbContextFactory<AssertManagerDbContext> _contextFactory;

    public SearchService(IDbContextFactory<AssertManagerDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<SearchResultDto>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return new List<SearchResultDto>();

        using var context = await _contextFactory.CreateDbContextAsync();
        var results = new List<SearchResultDto>();
        var term = query.ToLower();

        // 1. البحث في الأصول (سيريال أو باركود)
        var assets = await context.Assets
            .Include(a => a.Item)
            .Where(a => a.SerialNumber.Contains(term) || a.Barcode.Contains(term) || a.Item.Name.Contains(term))
            .Take(5) // كفاية 5 عشان الأداء
            .ToListAsync();

        results.AddRange(assets.Select(a => new SearchResultDto
        {
            Id = a.Id,
            Title = a.Item.Name,
            Description = $"S/N: {a.SerialNumber}",
            Type = "أصل / عهدة",
            Url = $"/assets", // ممكن توديه لصفحة التفاصيل لو عملتها
            Icon = Icons.Material.Filled.QrCodeScanner
        }));

        // 2. البحث في تذاكر الصيانة (رقم التذكرة أو الموضوع)
        // ملاحظة: الـ Id عبارة عن Guid فمش هينفع نعمل Contains عليه بسهولة، هنبحث في العنوان
        var tickets = await context.MaintenanceTickets
            .Include(t => t.Asset).ThenInclude(a => a.Item)
            .Where(t => t.Subject.Contains(term) || t.ReportedBy.Contains(term))
            .Take(5)
            .ToListAsync();

        results.AddRange(tickets.Select(t => new SearchResultDto
        {
            Id = t.Id,
            Title = $"صيانة: {t.Subject}",
            Description = $"{t.Asset.Item.Name} - {t.ReportedBy}",
            Type = "تذكرة صيانة",
            Url = $"/maintenance/details/{t.Id}",
            Icon = Icons.Material.Filled.Build
        }));

        // 3. البحث في الأصناف (اسم الصنف)
        var items = await context.Items
            .Where(i => i.Name.Contains(term) || i.Code.Contains(term))
            .Take(5)
            .ToListAsync();

        results.AddRange(items.Select(i => new SearchResultDto
        {
            Id = i.Id,
            Title = i.Name,
            Description = $"كود: {i.Code}",
            Type = "صنف مخزني",
            Url = "/items",
            Icon = Icons.Material.Filled.Category
        }));

        return results;
    }
}