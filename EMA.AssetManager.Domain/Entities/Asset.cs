using EMA.AssetManager.Domain.Entities.Common;
using EMA.AssetManager.Domain.Enums;

namespace EMA.AssetManager.Domain.Entities;

public class Asset : AuditableEntity<Guid>
{
    // البيانات التعريفية الفريدة
    public string SerialNumber { get; set; } = string.Empty; // السيريال (زي رقم الشاسيه)
    public string Barcode { get; set; } = string.Empty;      // الباركود (للمسح السريع)

    // الحالة
    public AssetStatus Status { get; set; } = AssetStatus.Available;

    // تواريخ مهمة
    public DateTime? PurchaseDate { get; set; }    // تاريخ الشراء
    public DateTime? ExpiryDate { get; set; }      // تاريخ انتهاء الضمان/الصلاحية

    // الربط بالأصناف (ده عبارة عن إيه؟)
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = default!;

    // الربط بالمخازن (موجود فين؟)
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public ICollection<AssetTransaction> AssetTransactions { get; set; } = [];
    public ICollection<MaintenanceTicket> MaintenanceTickets { get; set; } = [];
}
