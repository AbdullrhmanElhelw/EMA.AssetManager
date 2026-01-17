using EMA.AssetManager.Domain.Entities;
using Microsoft.AspNetCore.Components.Forms; // عشان رفع الملفات

namespace EMA.AssetManager.Services.Interfaces;

public interface ISettingsService
{
    event Action? OnChange;

    Task<SystemSetting> GetSettingsAsync();
    Task UpdateSettingsAsync(SystemSetting settings);

    // دالة رفع اللوجو الجديدة
    Task<string> UploadLogoAsync(IBrowserFile file);
}