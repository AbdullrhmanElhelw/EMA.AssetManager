using EMA.AssetManager.Domain.Data;
using EMA.AssetManager.Domain.Entities;
using EMA.AssetManager.Domain.Enums;
using EMA.AssetManager.Services.Dtos.Maintenance;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EMA.AssetManager.Services.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly IDbContextFactory<AssertManagerDbContext> _contextFactory;

    public MaintenanceService(IDbContextFactory<AssertManagerDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<MaintenanceTicketDto?> GetTicketByIdAsync(Guid id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var ticket = await context.MaintenanceTickets
            .Include(t => t.Asset)
            .ThenInclude(a => a.Item) // عشان نجيب اسم الصنف
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null) return null;

        return new MaintenanceTicketDto
        {
            Id = ticket.Id,
            AssetId = ticket.AssetId,
            AssetName = ticket.Asset.Item?.Name ?? "غير معروف",
            AssetSerialNumber = ticket.Asset.SerialNumber,
            Subject = ticket.Subject,
            // (تعديل: ضيفنا الخواص دي في الـ DTO لو مش موجودة، أو نستخدم Description هنا لو DTO ناقص)
            // Description = ticket.Description, 
            ReportedBy = ticket.ReportedBy,
            Priority = ticket.Priority,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            CompletedAt = ticket.CompletedAt,
            // 🔥 بيانات الإغلاق المهمة
            // تأكد إنك ضفت الخواص دي في MaintenanceTicketDto لو مش موجودة
            // TechnicianNotes = ticket.TechnicianNotes,
            // Cost = ticket.Cost
        };
    }
    public async Task<List<MaintenanceTicketDto>> GetAllTicketsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.MaintenanceTickets
            .Include(t => t.Asset)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new MaintenanceTicketDto
            {
                Id = t.Id,
                AssetId = t.AssetId,
                AssetName = t.Asset.Item.Name, // تأكد إنك عامل Include للـ Item في الـ Context لو احتجت
                AssetSerialNumber = t.Asset.SerialNumber,
                Subject = t.Subject,
                ReportedBy = t.ReportedBy,
                Priority = t.Priority,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt
            })
            .ToListAsync();
    }

    public async Task<List<MaintenanceTicketDto>> GetActiveTicketsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.MaintenanceTickets
            .Include(t => t.Asset)
            .Where(t => t.Status != TicketStatus.Resolved && t.Status != TicketStatus.Unrepairable)
            .OrderByDescending(t => t.Priority) // الأهم فالأهم
            .Select(t => new MaintenanceTicketDto
            {
                Id = t.Id,
                AssetId = t.AssetId,
                AssetName = t.Asset.Item.Name,
                AssetSerialNumber = t.Asset.SerialNumber,
                Subject = t.Subject,
                ReportedBy = t.ReportedBy,
                Priority = t.Priority,
                Status = t.Status,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<Guid> CreateTicketAsync(CreateTicketDto dto)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // 1. إنشاء التذكرة
        var ticket = new MaintenanceTicket
        {
            Id = Guid.NewGuid(),
            AssetId = dto.AssetId,
            Subject = dto.Subject,
            Description = dto.Description,
            ReportedBy = dto.ReportedBy,
            Priority = dto.Priority,
            Status = TicketStatus.Open,
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        context.MaintenanceTickets.Add(ticket);

        // 2. 🔥 تحديث حالة الأصل لـ "تحت الصيانة" أوتوماتيك
        var asset = await context.Assets.FindAsync(dto.AssetId);
        if (asset != null)
        {
            asset.Status = AssetStatus.UnderMaintenance;
            context.Assets.Update(asset);
        }

        await context.SaveChangesAsync();
        return ticket.Id;
    }

    public async Task ResolveTicketAsync(ResolveTicketDto dto)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var ticket = await context.MaintenanceTickets.FindAsync(dto.TicketId);
        if (ticket == null) throw new Exception("التذكرة غير موجودة");

        // 1. تحديث بيانات التذكرة
        ticket.TechnicianNotes = dto.TechnicianNotes;
        ticket.Cost = dto.Cost;
        ticket.CompletedAt = DateTime.UtcNow;

        // لو الجهاز تالف، نقفل التذكرة بـ Unrepairable
        ticket.Status = (dto.FinalAssetStatus == AssetStatus.Damaged || dto.FinalAssetStatus == AssetStatus.Retired)
            ? TicketStatus.Unrepairable
            : TicketStatus.Resolved;

        // 2. 🔥 تحديث حالة الأصل النهائية (متاح أو تالف)
        var asset = await context.Assets.FindAsync(ticket.AssetId);
        if (asset != null)
        {
            asset.Status = dto.FinalAssetStatus;
            context.Assets.Update(asset);
        }

        await context.SaveChangesAsync();
    }
}