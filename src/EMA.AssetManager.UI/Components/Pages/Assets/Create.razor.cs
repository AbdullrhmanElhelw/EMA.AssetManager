using EMA.AssetManager.Domain.Enums;
using EMA.AssetManager.Services.Dtos.Assets;
using EMA.AssetManager.Services.Dtos.Items;
using EMA.AssetManager.Services.Dtos.Warehouses;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages.Assets;

public partial class Create : ComponentBase
{
    [Inject] private IAssetService AssetService { get; set; } = default!;
    [Inject] private IItemService ItemService { get; set; } = default!;
    [Inject] private IWarehouseService WarehouseService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private CreateAssetDto _model = new();
    private MudForm _form = default!;
    private bool _processing = false;

    // قوائم الاختيار
    private List<ItemDto> _items = new();
    private List<WarehouseDto> _warehouses = new();

    // متغيرات مساعدة للـ Dropdowns
    private Guid? _selectedItemId;
    private Guid? _selectedWarehouseId;

    protected override async Task OnInitializedAsync()
    {
        await LoadInitialData();
    }

    private async Task LoadInitialData()
    {
        try
        {
            // تحميل الأصناف والمخازن بالتتابع
            var itemsList = await ItemService.GetItemsAsync();
            _items = itemsList.ToList(); // ممكن هنا نفلتر الأصناف النشطة لو عندك خاصية IsActive

            var warehousesList = await WarehouseService.GetWarehousesAsync();
            _warehouses = warehousesList.Where(w => w.IsActive).ToList(); // بنعرض المخازن النشطة بس
        }
        catch (Exception ex)
        {
            Snackbar.Add($"فشل تحميل البيانات: {ex.Message}", Severity.Warning);
        }
    }

    private async Task Submit()
    {
        await _form.Validate();

        // التحقق اليدوي من الاختيارات
        if (_selectedItemId == null)
        {
            Snackbar.Add("يجب اختيار الصنف", Severity.Error);
            return;
        }
        if (_selectedWarehouseId == null)
        {
            Snackbar.Add("يجب اختيار المخزن", Severity.Error);
            return;
        }

        if (!_form.IsValid) return;

        _processing = true;
        try
        {
            // ربط الاختيارات بالـ Model
            _model.ItemId = _selectedItemId.Value;
            _model.WarehouseId = _selectedWarehouseId.Value;

            await AssetService.CreateAssetAsync(_model);

            Snackbar.Add("تم إضافة العهدة بنجاح", Severity.Success);

            // هنا ممكن نخيره: عايز يضيف قطعة كمان ولا يرجع للقائمة؟
            // حالياً هنرجعه للقائمة
            Navigation.NavigateTo("/assets");
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            _processing = false;
        }
    }

    private void Cancel() => Navigation.NavigateTo("/assets");

    // دالة مساعدة لترجمة الحالة في القائمة
    private string GetStatusName(AssetStatus status)
    {
        return status switch
        {
            AssetStatus.Available => "متاح (جديد)",
            AssetStatus.InUse => "مستخدم / مصروف",
            AssetStatus.UnderMaintenance => "تحت الصيانة",
            AssetStatus.Damaged => "تالف",
            AssetStatus.Retired => "كهنة",
            _ => status.ToString()
        };
    }
}