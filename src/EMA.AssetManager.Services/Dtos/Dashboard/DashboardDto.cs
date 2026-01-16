namespace EMA.AssetManager.Services.Dtos.Dashboard;

public class DashboardDto
{
    // إحصائيات عامة
    public int TotalCategories { get; set; }
    public int TotalItems { get; set; }
    public int TotalWarehouses { get; set; }
    public int TotalAssets { get; set; } // إجمالي قطع العهدة

    // تفاصيل حالة العهدة (عشان الرسم البياني أو الكروت)
    public int AssetsAvailable { get; set; }        // متاح
    public int AssetsInUse { get; set; }            // مستخدم
    public int AssetsUnderMaintenance { get; set; } // صيانة
    public int AssetsDamaged { get; set; }          // تالف

    // تنبيهات
    public int LowStockItemsCount { get; set; } // عدد الأصناف اللي قربت تخلص (مثلاً أقل من 5)
}