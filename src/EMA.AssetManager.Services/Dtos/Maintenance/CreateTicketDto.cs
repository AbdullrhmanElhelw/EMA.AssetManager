using EMA.AssetManager.Domain.Enums;

namespace EMA.AssetManager.Services.Dtos.Maintenance;

public class CreateTicketDto
{
    public Guid AssetId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportedBy { get; set; } = string.Empty; // اسم المبلغ
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
}
