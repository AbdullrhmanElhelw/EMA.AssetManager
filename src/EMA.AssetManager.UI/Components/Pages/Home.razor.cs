using EMA.AssetManager.Services.Dtos.Dashboard;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages;

// لازم الكلاس يكون partial عشان يتربط بملف التصميم
public partial class Home : ComponentBase
{
    // الحقن هنا بيتم عن طريق [Inject] بدل @inject
    [Inject] private IDashboardService DashboardService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    // المتغيرات
    private DashboardDto _data = new();
    private bool _isLoading = true;

    // إعدادات الرسم البياني
    private double[] _chartData = Array.Empty<double>();
    private string[] _chartLabels = { "متاح", "مستخدم", "صيانة", "تالف" };
    private ChartOptions _chartOptions = new ChartOptions
    {
        ChartPalette = new[] { "#00C853", "#2196F3", "#FF9800", "#F44336" }
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
            // استدعاء الخدمة السريعة (اللي عدلناها في الخطوة السابقة)
            _data = await DashboardService.GetDashboardDataAsync();

            // تجهيز بيانات الرسم البياني
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
            Snackbar.Add($"فشل تحميل الإحصائيات: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }
}