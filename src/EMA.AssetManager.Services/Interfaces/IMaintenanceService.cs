using EMA.AssetManager.Services.Dtos.Maintenance;

namespace EMA.AssetManager.Services.Interfaces;

public interface IMaintenanceService
{
    Task<MaintenanceTicketDto?> GetTicketByIdAsync(Guid id);
    Task<List<MaintenanceTicketDto>> GetAllTicketsAsync();
    Task<List<MaintenanceTicketDto>> GetActiveTicketsAsync(); // التذاكر المفتوحة فقط

    // العمليات
    Task<Guid> CreateTicketAsync(CreateTicketDto dto); // فتح بلاغ
    Task ResolveTicketAsync(ResolveTicketDto dto); // إغلاق بلاغ
}
