using EMA.AssetManager.Domain.Entities.Common;

namespace EMA.AssetManager.Domain.Entities;

public class AssetTransaction : AuditableEntity<Guid>
{
    public Guid AssetId { get; set; }
    public Asset Asset { get; set; } = default!;

    // نوع الحركة: صرف (Issue) ولا إرجاع (Return)
    public TransactionType TransactionType { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    // اسم الشخص المستلم (ممكن يبقى نص حالياً، أو تربطه بجدول موظفين لو عندك)
    public string? RecipientName { get; set; }

    // المخزن اللي تمت فيه الحركة
    public Guid? WarehouseId { get; set; }

    public Warehouse? Warehouse { get; set; }

    // ملاحظات (مثلاً: رجع مكسور، صرف لمأمورية..)
    public string? Notes { get; set; }
}
