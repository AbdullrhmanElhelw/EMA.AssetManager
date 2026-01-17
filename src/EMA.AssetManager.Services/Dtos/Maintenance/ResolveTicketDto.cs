using EMA.AssetManager.Domain.Enums;

namespace EMA.AssetManager.Services.Dtos.Maintenance;

public class ResolveTicketDto
{
    public Guid TicketId { get; set; }
    public string TechnicianNotes { get; set; } = string.Empty; // تقرير الفني
    public decimal Cost { get; set; } = 0; // التكلفة

    // الحالة النهائية للأصل بعد الإصلاح (متاح / تالف)
    // لو تم الإصلاح بنجاح، هنخلي الجهاز Available
    // لو فشل، هنخليه Damaged
    public AssetStatus FinalAssetStatus { get; set; } = AssetStatus.Available;
}