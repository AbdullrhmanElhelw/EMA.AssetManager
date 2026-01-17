using EMA.AssetManager.Domain.Enums;

namespace EMA.AssetManager.Services.Dtos.Maintenance;

public class MaintenanceTicketDto
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public string AssetSerialNumber { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; // الوصف
    public string ReportedBy { get; set; } = string.Empty;

    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }

    // تفاصيل الإغلاق
    public string? TechnicianNotes { get; set; } // تقرير الفني
    public decimal Cost { get; set; } // التكلفة

    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}