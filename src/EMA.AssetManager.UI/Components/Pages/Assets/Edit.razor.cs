using EMA.AssetManager.Domain.Enums;
using EMA.AssetManager.Services.Dtos.Assets;
using EMA.AssetManager.Services.Dtos.Warehouses;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages.Assets;

public partial class Edit : ComponentBase
{
    [Inject] private IAssetService AssetService { get; set; } = default!;
    [Inject] private IWarehouseService WarehouseService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    [Parameter] public Guid Id { get; set; }

    private UpdateAssetDto _model = new();
    private MudForm _form = default!;
    private bool _processing = false;
    private bool _isLoading = true;

    // بنحتاج قائمة المخازن عشان لو هننقل العهدة
    private List<WarehouseDto> _warehouses = new();

    // عشان نعرض اسم الصنف للمستخدم (للقراءة فقط)
    private string _itemName = "";
    private string _itemCode = "";

    // متغير مساعد للـ Dropdown
    private Guid? _selectedWarehouseId;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        _isLoading = true;
        try
        {
            // 1. تحميل المخازن (النشطة فقط)
            var warehousesList = await WarehouseService.GetWarehousesAsync();
            _warehouses = warehousesList.Where(w => w.IsActive).ToList();

            // 2. تحميل بيانات الأصل
            var asset = await AssetService.GetAssetByIdAsync(Id);

            if (asset == null)
            {
                Snackbar.Add("العهدة غير موجودة", Severity.Error);
                Navigation.NavigateTo("/assets");
                return;
            }

            // 3. تخزين بيانات للعرض فقط
            _itemName = asset.ItemName;
            _itemCode = asset.ItemCode;

            // 4. تعبئة الـ DTO
            _model = new UpdateAssetDto
            {
                SerialNumber = asset.SerialNumber,
                Barcode = asset.Barcode,
                Status = asset.Status,
                PurchaseDate = asset.PurchaseDate,
                ExpiryDate = asset.ExpiryDate,
                WarehouseId = asset.WarehouseId
            };

            // ضبط المخزن المختار
            _selectedWarehouseId = asset.WarehouseId;
        }
        catch (Exception ex)
        {
            Snackbar.Add($"فشل تحميل البيانات: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task Submit()
    {
        await _form.Validate();

        if (_selectedWarehouseId == null)
        {
            Snackbar.Add("يجب تحديد المخزن", Severity.Error);
            return;
        }

        if (!_form.IsValid) return;

        _processing = true;
        try
        {
            _model.WarehouseId = _selectedWarehouseId.Value;

            await AssetService.UpdateAssetAsync(Id, _model);

            Snackbar.Add("تم تعديل العهدة بنجاح", Severity.Success);
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

    private string GetStatusName(AssetStatus status)
    {
        return status switch
        {
            AssetStatus.Available => "متاح بالمخزن",
            AssetStatus.InUse => "مستخدم / مصروف",
            AssetStatus.UnderMaintenance => "تحت الصيانة",
            AssetStatus.Damaged => "تالف",
            AssetStatus.Retired => "تكهين",
            _ => status.ToString()
        };
    }
}