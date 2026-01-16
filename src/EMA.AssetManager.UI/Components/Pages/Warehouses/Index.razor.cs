using EMA.AssetManager.Services.Dtos.Warehouses;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages.Warehouses;

public partial class Index : ComponentBase
{
    [Inject] private IWarehouseService WarehouseService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;

    private List<WarehouseDto>? _warehouses;
    private bool _isLoading = true;

    // --- متغيرات الفلاتر ---
    private string _searchString = "";
    private int _statusFilter = 0; // 0=الكل، 1=نشط، 2=غير نشط
    private string? _selectedKeeperFilter; // فلتر اسم الأمين
    private List<string> _keepersList = new(); // قائمة الأمناء الموجودين لاستخدامها في الفلتر

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        _isLoading = true;
        try
        {
            var result = await WarehouseService.GetWarehousesAsync();
            _warehouses = result.ToList();

            // استخراج قائمة الأمناء المميزة (عشان الفلتر)
            _keepersList = _warehouses
                .Where(w => !string.IsNullOrEmpty(w.KeeperName))
                .Select(w => w.KeeperName!)
                .Distinct()
                .OrderBy(k => k)
                .ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"خطأ: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }

    // دالة الفلتر الشاملة
    private bool FilterFunc(WarehouseDto element)
    {
        // 1. البحث النصي (اسم، موقع، أمين)
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            var searchData = $"{element.Name} {element.Location} {element.KeeperName}";
            if (!searchData.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        // 2. فلتر الحالة
        if (_statusFilter == 1 && !element.IsActive) return false; // نشط فقط
        if (_statusFilter == 2 && element.IsActive) return false;  // غير نشط فقط

        // 3. فلتر أمين المخزن
        if (!string.IsNullOrEmpty(_selectedKeeperFilter))
        {
            if (element.KeeperName != _selectedKeeperFilter)
                return false;
        }

        return true;
    }

    private async Task ToggleStatus(WarehouseDto warehouse, bool newStatus)
    {
        try
        {
            var updateDto = new UpdateWarehouseDto
            {
                Name = warehouse.Name,
                Location = warehouse.Location,
                KeeperName = warehouse.KeeperName,
                IsActive = newStatus
            };

            await WarehouseService.UpdateWarehouseAsync(warehouse.Id, updateDto);
            warehouse.IsActive = newStatus;
            Snackbar.Add($"تم تغيير الحالة بنجاح", Severity.Success);
        }
        catch (Exception ex)
        {
            await LoadData(); // Revert changes on error
            Snackbar.Add($"فشل التغيير: {ex.Message}", Severity.Error);
        }
    }

    private void NavigateToCreate() => Navigation.NavigateTo("/warehouses/create");
    private void NavigateToEdit(Guid id) => Navigation.NavigateTo($"/warehouses/edit/{id}");

    private async Task ShowDeleteDialog(WarehouseDto warehouse)
    {
        var result = await DialogService.ShowMessageBox(
            "تأكيد الحذف",
            $"هل أنت متأكد من حذف مخزن '{warehouse.Name}'؟",
            yesText: "حذف", cancelText: "إلغاء",
            options: new DialogOptions { FullWidth = true, MaxWidth = MaxWidth.ExtraSmall });

        if (result == true)
        {
            await DeleteWarehouse(warehouse.Id);
        }
    }

    private async Task DeleteWarehouse(Guid id)
    {
        try
        {
            await WarehouseService.DeleteWarehouseAsync(id);
            Snackbar.Add("تم الحذف بنجاح", Severity.Success);
            await LoadData();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
}