using EMA.AssetManager.Services.Dtos.Warehouses;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages.Warehouses;

public partial class Create : ComponentBase
{
    [Inject] private IWarehouseService WarehouseService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private CreateWarehouseDto _model = new();
    private MudForm _form = default!;
    private bool _processing = false;

    private async Task Submit()
    {
        await _form.Validate();
        if (!_form.IsValid) return;

        _processing = true;
        try
        {
            await WarehouseService.CreateWarehouseAsync(_model);
            Snackbar.Add("تم إضافة المخزن بنجاح", Severity.Success);
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