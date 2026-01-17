using Microsoft.AspNetCore.Components;

namespace EMA.AssetManager.UI.Components.Layout;

public partial class NavMenu : ComponentBase
{
    [Parameter]
    public bool IsDrawerOpen { get; set; } = true;

    // Menu States
    private bool _isDashboardExpanded = true;
    private bool _isDefinitionsExpanded = false;
    private bool _isOperationsExpanded = false;
    private bool _isReportsExpanded = false;

    // Icon Rotations
    private string DashboardIconStyle => $"transform: {(_isDashboardExpanded ? "rotate(0deg)" : "rotate(90deg)")}; transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);";
    private string DefinitionsIconStyle => $"transform: {(_isDefinitionsExpanded ? "rotate(0deg)" : "rotate(90deg)")}; transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);";
    private string OperationsIconStyle => $"transform: {(_isOperationsExpanded ? "rotate(0deg)" : "rotate(90deg)")}; transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);";
    private string ReportsIconStyle => $"transform: {(_isReportsExpanded ? "rotate(0deg)" : "rotate(90deg)")}; transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);";

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

    private void CollapseOtherSections(string currentSection)
    {
        if (currentSection != "dashboard") _isDashboardExpanded = false;
        if (currentSection != "definitions") _isDefinitionsExpanded = false;
        if (currentSection != "operations") _isOperationsExpanded = false;
        if (currentSection != "reports") _isReportsExpanded = false;

        StateHasChanged();
    }
}