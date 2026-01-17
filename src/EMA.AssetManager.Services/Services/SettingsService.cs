using EMA.AssetManager.Domain.Data;
using EMA.AssetManager.Domain.Entities;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EMA.AssetManager.Services.Services;

public class SettingsService : ISettingsService
{
    private readonly IDbContextFactory<AssertManagerDbContext> _contextFactory;
    private readonly IWebHostEnvironment _env; // بيئة الاستضافة

    public event Action? OnChange;

    public SettingsService(IDbContextFactory<AssertManagerDbContext> contextFactory, IWebHostEnvironment env)
    {
        _contextFactory = contextFactory;
        _env = env;
    }

    public async Task<SystemSetting> GetSettingsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var settings = await context.SystemSettings.FirstOrDefaultAsync();

        if (settings == null)
        {
            settings = new SystemSetting
            {
                Id = Guid.NewGuid(),
                CompanyName = "الأكاديمية العسكرية المصرية",
                BranchName = "الفرع الرئيسي",
                PrimaryColor = "#1E3A2F",
                SecondaryColor = "#C5A059",
                DrawerBackgroundColor = "#1A2B26",
                DrawerTextColor = "#FFFFFF",
                LowStockThreshold = 5,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow
            };
            context.SystemSettings.Add(settings);
            await context.SaveChangesAsync();
        }
        else
        {
            // تصحيح القيم الفارغة (Defensive Coding)
            bool changed = false;
            if (string.IsNullOrWhiteSpace(settings.DrawerBackgroundColor)) { settings.DrawerBackgroundColor = "#1A2B26"; changed = true; }
            if (string.IsNullOrWhiteSpace(settings.DrawerTextColor)) { settings.DrawerTextColor = "#FFFFFF"; changed = true; }
            if (string.IsNullOrWhiteSpace(settings.PrimaryColor)) { settings.PrimaryColor = "#1E3A2F"; changed = true; }

            if (changed)
            {
                context.SystemSettings.Update(settings);
                await context.SaveChangesAsync();
            }
        }

        return settings;
    }

    public async Task UpdateSettingsAsync(SystemSetting settings)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.SystemSettings.FirstOrDefaultAsync();

        if (existing != null)
        {
            existing.CompanyName = settings.CompanyName;
            existing.BranchName = settings.BranchName;
            existing.Currency = settings.Currency;
            existing.LogoPath = settings.LogoPath; // تحديث اللوجو

            existing.PrimaryColor = settings.PrimaryColor;
            existing.SecondaryColor = settings.SecondaryColor;
            existing.DrawerBackgroundColor = settings.DrawerBackgroundColor;
            existing.DrawerTextColor = settings.DrawerTextColor;

            existing.LowStockThreshold = settings.LowStockThreshold;
            existing.EnableEmailNotifications = settings.EnableEmailNotifications;
            existing.LastModifiedAt = DateTime.UtcNow;

            context.SystemSettings.Update(existing);
            await context.SaveChangesAsync();

            OnChange?.Invoke(); // تبليغ السيستم
        }
    }

    public async Task<string> UploadLogoAsync(IBrowserFile file)
    {
        if (file == null) return null;

        // مسار الحفظ: wwwroot/uploads
        var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

        // اسم فريد
        var fileName = $"logo_{DateTime.Now.Ticks}{Path.GetExtension(file.Name)}";
        var filePath = Path.Combine(uploadPath, fileName);

        // الحفظ
        await using var stream = new FileStream(filePath, FileMode.Create);
        await using var fs = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024); // 5MB Max
        await fs.CopyToAsync(stream);

        return $"/uploads/{fileName}";
    }
}