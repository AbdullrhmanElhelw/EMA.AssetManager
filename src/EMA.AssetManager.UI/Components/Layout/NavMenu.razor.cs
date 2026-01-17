using Microsoft.AspNetCore.Components;

namespace EMA.AssetManager.UI.Components.Layout;

public partial class NavMenu : ComponentBase
{
    [Parameter]
    public bool IsDrawerOpen { get; set; } = true;

    // حالة فتح القوائم (الرئيسية مفتوحة افتراضياً)
    private bool _isDashboardExpanded = true;
    private bool _isDefinitionsExpanded = false; // قسم التعريفات
    private bool _isOperationsExpanded = false;  // قسم العمليات
    private bool _isReportsExpanded = false;     // قسم التقارير

    // أنماط الأيقونات (حركة الدوران للسهم)
    private string DashboardIconStyle => $"transform: {(_isDashboardExpanded ? "rotate(0deg)" : "rotate(-90deg)")}; transition: transform 0.3s;";
    private string DefinitionsIconStyle => $"transform: {(_isDefinitionsExpanded ? "rotate(0deg)" : "rotate(-90deg)")}; transition: transform 0.3s;";
    private string OperationsIconStyle => $"transform: {(_isOperationsExpanded ? "rotate(0deg)" : "rotate(-90deg)")}; transition: transform 0.3s;";
    private string ReportsIconStyle => $"transform: {(_isReportsExpanded ? "rotate(0deg)" : "rotate(-90deg)")}; transition: transform 0.3s;";

    private void ToggleDashboardSection()
    {
        _isDashboardExpanded = !_isDashboardExpanded;
        if (_isDashboardExpanded) CollapseOtherSections("dashboard");
    }

    private void ToggleDefinitionsSection()
    {
        _isDefinitionsExpanded = !_isDefinitionsExpanded;
        if (_isDefinitionsExpanded) CollapseOtherSections("definitions");
    }

    private void ToggleOperationsSection()
    {
        _isOperationsExpanded = !_isOperationsExpanded;
        if (_isOperationsExpanded) CollapseOtherSections("operations");
    }

    private void ToggleReportsSection()
    {
        _isReportsExpanded = !_isReportsExpanded;
        if (_isReportsExpanded) CollapseOtherSections("reports");
    }

    // دالة مساعدة لغلق باقي الأقسام عند فتح قسم جديد (للحفاظ على نظافة القائمة)
    private void CollapseOtherSections(string currentSection)
    {
        if (currentSection != "dashboard") _isDashboardExpanded = false;
        if (currentSection != "definitions") _isDefinitionsExpanded = false;
        if (currentSection != "operations") _isOperationsExpanded = false;
        if (currentSection != "reports") _isReportsExpanded = false;

        StateHasChanged();
    }
}