using EMA.AssetManager.Services.Dtos.Reports;

public interface IReportService
{
    Task<List<InventoryReportDto>> GetInventorySummaryAsync();
}