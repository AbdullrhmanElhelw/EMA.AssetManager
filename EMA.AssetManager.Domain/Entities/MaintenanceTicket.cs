using EMA.AssetManager.Domain.Entities.Common;
using EMA.AssetManager.Domain.Enums;

namespace EMA.AssetManager.Domain.Entities;

public class MaintenanceTicket : AuditableEntity<Guid>
{
    // ربط التذكرة بالأصل
    public Guid AssetId { get; set; }
    public Asset Asset { get; set; } = default!;

    // تفاصيل البلاغ
    public string Subject { get; set; } = string.Empty; // عنوان المشكلة (مثلاً: الشاشة لا تعمل)
    public string Description { get; set; } = string.Empty; // وصف تفصيلي
    public string ReportedBy { get; set; } = string.Empty; // اسم الشخص اللي بلغ عن العطل

    // الحالة والأولوية
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public TicketStatus Status { get; set; } = TicketStatus.Open;

    // تفاصيل الإصلاح (يتم ملؤها بواسطة الفني)
    public string? TechnicianNotes { get; set; } // تقرير الفني
    public decimal Cost { get; set; } = 0; // تكلفة الإصلاح (قطع غيار + مصنعية)

    // التواريخ
    public DateTime? StartedAt { get; set; } // متى بدأ الفني العمل
    public DateTime? CompletedAt { get; set; } // متى تم إغلاق التذكرة
}
