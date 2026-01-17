using EMA.AssetManager.Domain.Entities.Common;

namespace EMA.AssetManager.Domain.Entities;

public class SystemSetting : AuditableEntity<Guid>
{
    // 1. الهوية واللوجو
    public string CompanyName { get; set; } = "الأكاديمية العسكرية المصرية";
    public string BranchName { get; set; } = "نظام إدارة الأصول";
    public string? LogoPath { get; set; } // مسار اللوجو الجديد
    public string Currency { get; set; } = "EGP";

    // 2. الألوان الرئيسية
    public string PrimaryColor { get; set; } = "#1E3A2F";
    public string SecondaryColor { get; set; } = "#C5A059";

    // 3. ألوان القائمة الجانبية
    public string DrawerBackgroundColor { get; set; } = "#1A2B26";
    public string DrawerTextColor { get; set; } = "#FFFFFF";

    // 4. التنبيهات
    public int LowStockThreshold { get; set; } = 5;
    public bool EnableEmailNotifications { get; set; } = false;
}