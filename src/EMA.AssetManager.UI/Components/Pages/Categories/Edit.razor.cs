using EMA.AssetManager.Services.Dtos.Categories.Create;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages.Categories;

public partial class Edit : ComponentBase
{
    [Inject] private ICategoryService CategoryService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    // الـ ID جاي من الرابط
    [Parameter] public Guid Id { get; set; }

    private UpdateCategoryDto _model = new();
    private MudForm _form = default!;
    private bool _processing = false;
    private bool _isLoadingData = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            // بنجيب البيانات الحالية عشان نعرضها في الفورم
            var category = await CategoryService.GetCategoryByIdAsync(Id);

            if (category == null)
            {
                Snackbar.Add("هذه الفئة غير موجودة", Severity.Error);
                Navigation.NavigateTo("/categories");
                return;
            }

            // بنحول الـ CategoryDto لـ UpdateCategoryDto
            _model = new UpdateCategoryDto
            {
                Name = category.Name,
                IsActive = category.IsActive
            };
        }
        catch (Exception ex)
        {
            Snackbar.Add($"حدث خطأ أثناء تحميل البيانات: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isLoadingData = false;
        }
    }

    private async Task Submit()
    {
        await _form.Validate();

        if (!_form.IsValid)
            return;

        _processing = true;
        try
        {
            // إرسال التعديل للسرفيس
            await CategoryService.UpdateCategoryAsync(Id, _model);

            Snackbar.Add("تم تعديل الفئة بنجاح", Severity.Success);
            Navigation.NavigateTo("/categories");
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