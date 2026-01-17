namespace EMA.AssetManager.Services.Dtos.Reports;

public class InventoryReportDto
{
    public string CategoryName { get; set; } = string.Empty; // اسم الفئة (لابتوب، طابعات...)
    public int TotalItemsCount { get; set; } // عدد الموديلات
    public int TotalAssetsCount { get; set; } // عدد القطع الفعلي (سيريال)
    public int AvailableCount { get; set; } // المتاح في المخزن
    public int AssignedCount { get; set; } // المصروف
    public int MaintenanceCount { get; set; } // في الصيانة
    public int DamagedCount { get; set; } // كهنة
}