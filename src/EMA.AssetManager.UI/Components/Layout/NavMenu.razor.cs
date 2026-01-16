using Microsoft.AspNetCore.Components;

namespace EMA.AssetManager.UI.Components.Layout;

public partial class NavMenu : ComponentBase
{
    [Parameter]
    public bool IsDrawerOpen { get; set; } = true;

    // الأقسام تكون مقفولة افتراضياً
    private bool _isDashboardExpanded = false;
    private bool _isManagementExpanded = false;
    private bool _isReportsExpanded = false;

    private string DashboardIconStyle => $"transform: {(_isDashboardExpanded ? "rotate(0deg)" : "rotate(-90deg)")}; transition: transform 0.3s;";
    private string ManagementIconStyle => $"transform: {(_isManagementExpanded ? "rotate(0deg)" : "rotate(-90deg)")}; transition: transform 0.3s;";
    private string ReportsIconStyle => $"transform: {(_isReportsExpanded ? "rotate(0deg)" : "rotate(-90deg)")}; transition: transform 0.3s;";

    private void ToggleDashboardSection()
    {
        _isDashboardExpanded = !_isDashboardExpanded;

        // إغلاق الأقسام الأخرى
        if (_isDashboardExpanded)
        {
            _isManagementExpanded = false;
            _isReportsExpanded = false;
        }

        StateHasChanged();
    }

    private void ToggleManagementSection()
    {
        _isManagementExpanded = !_isManagementExpanded;

        // إغلاق الأقسام الأخرى
        if (_isManagementExpanded)
        {
            _isDashboardExpanded = false;
            _isReportsExpanded = false;
        }

        StateHasChanged();
    }

    private void ToggleReportsSection()
    {
        _isReportsExpanded = !_isReportsExpanded;

        // إغلاق الأقسام الأخرى
        if (_isReportsExpanded)
        {
            _isDashboardExpanded = false;
            _isManagementExpanded = false;
        }

        StateHasChanged();
    }

    // دالة لفتح قسم معين من الخارج (مثلاً من الصفحة الحالية)
    public void ExpandSection(string sectionName)
    {
        // إغلاق جميع الأقسام أولاً
        _isDashboardExpanded = false;
        _isManagementExpanded = false;
        _isReportsExpanded = false;

        // فتح القسم المطلوب
        switch (sectionName.ToLower())
        {
            case "dashboard":
            case "الرئيسية":
                _isDashboardExpanded = true;
                break;
            case "management":
            case "إدارة العهدة":
                _isManagementExpanded = true;
                break;
            case "reports":
            case "التقارير والإدارة":
                _isReportsExpanded = true;
                break;
        }

        StateHasChanged();
    }
}