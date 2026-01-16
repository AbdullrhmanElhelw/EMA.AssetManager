using EMA.AssetManager.Services.Dtos.Warehouses;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages.Warehouses;

public partial class Edit : ComponentBase
{
    [Inject] private IWarehouseService WarehouseService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    [Parameter] public Guid Id { get; set; }

    private UpdateWarehouseDto _model = new();
    private MudForm _form = default!;
    private bool _processing = false;
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            var warehouse = await WarehouseService.GetWarehouseByIdAsync(Id);
            if (warehouse == null)
            {
                Snackbar.Add("المخزن غير موجود", Severity.Error);
                Navigation.NavigateTo("/warehouses");
                return;
            }

            // نقل البيانات للـ DTO
            _model = new UpdateWarehouseDto
            {
                Name = warehouse.Name,
                Location = warehouse.Location,
                KeeperName = warehouse.KeeperName,
                IsActive = warehouse.IsActive
            };
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
        if (!_form.IsValid) return;

        _processing = true;
        try
        {
            await WarehouseService.UpdateWarehouseAsync(Id, _model);
            Snackbar.Add("تم تعديل بيانات المخزن بنجاح", Severity.Success);
            Navigation.NavigateTo("/warehouses");
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

    private void Cancel() => Navigation.NavigateTo("/warehouses");
}