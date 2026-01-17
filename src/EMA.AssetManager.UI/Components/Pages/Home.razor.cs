using EMA.AssetManager.Services.Dtos.Dashboard;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages;

public partial class Home : ComponentBase
{
    [Inject] private IDashboardService DashboardService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    // متغيرات
    private DashboardDto _data = new();
    private bool _isLoading = true;

    // إعدادات الرسم البياني
    private double[] _chartData = Array.Empty<double>();
    private string[] _chartLabels = { "متاح", "مستخدم", "صيانة", "تالف" };

    // 🔥 ألوان متناسقة مع الكروت (Success, Info, Warning, Error)
    private ChartOptions _chartOptions = new ChartOptions
    {
        ChartPalette = new[] { "#00C853", "#2196F3", "#FF9800", "#F44336" },
        LineStrokeWidth = 5
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        _isLoading = true;
        try
        {
            _data = await DashboardService.GetDashboardDataAsync();

            _chartData = new double[]
            {
                _data.AssetsAvailable,
                _data.AssetsInUse,
                _data.AssetsUnderMaintenance,
                _data.AssetsDamaged
            };
        }
        catch (Exception ex)
        {
            Snackbar.Add($"حدث خطأ أثناء تحميل البيانات: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }
}