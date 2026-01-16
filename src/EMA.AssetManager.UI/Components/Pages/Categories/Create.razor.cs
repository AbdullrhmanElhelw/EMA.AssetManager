using EMA.AssetManager.Services.Dtos.Categories.Create;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages.Categories;

public partial class Create : ComponentBase
{
    [Inject] private ICategoryService CategoryService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private CreateCategoryDto _model = new();
    private MudForm _form = default!;
    private bool _processing = false;

    private async Task Submit()
    {
        await _form.Validate();

        if (!_form.IsValid)
            return;

        _processing = true;
        try
        {
            await CategoryService.CreateCategoryAsync(_model);
            Snackbar.Add("تم إضافة الفئة بنجاح", Severity.Success);
            Navigation.NavigateTo("/categories"); // الرجوع للقائمة
        }
        catch (Exception ex)
        {
            Snackbar.Add($"حدث خطأ: {ex.Message}", Severity.Error);
        }
        finally
        {
            _processing = false;
        }
    }

    private void Cancel()
    {
        Navigation.NavigateTo("/categories");
    }
}