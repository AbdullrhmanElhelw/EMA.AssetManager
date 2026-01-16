using EMA.AssetManager.Services.Dtos.Dashboard;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default);
}