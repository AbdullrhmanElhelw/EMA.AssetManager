using EMA.AssetManager.Services.Dtos.Categories;
using EMA.AssetManager.Services.Dtos.Items;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages.Items;

public partial class Create : ComponentBase
{
    [Inject] private IItemService ItemService { get; set; } = default!;
    [Inject] private ICategoryService CategoryService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private CreateItemDto _model = new();
    private MudForm _form = default!;
    private bool _processing = false;
    private List<CategoryDto> _categories = new();

    // 1. ضفنا المتغير ده عشان يقبل null ويحل مشكلة الأصفار
    private Guid? _selectedCategoryId;

    protected override async Task OnInitializedAsync()
    {
        await LoadInitialData();
    }

    private async Task LoadInitialData()
    {
        try
        {
            var cats = await CategoryService.GetCategoriesAsync();
            _categories = cats.Where(c => c.IsActive).ToList();
            _model.Code = await ItemService.GenerateNextCodeAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"فشل تحميل البيانات الأولية: {ex.Message}", Severity.Warning);
        }
    }

    private async Task Submit()
    {
        await _form.Validate();

        // التحقق اليدوي إن المستخدم اختار فئة
        if (_selectedCategoryId == null)
        {
            Snackbar.Add("يجب اختيار الفئة", Severity.Error);
            return;
        }

        if (!_form.IsValid) return;

        _processing = true;
        try
        {
            // 2. بننقل القيمة المختارة للـ Model قبل الإرسال
            _model.CategoryId = _selectedCategoryId.Value;

            await ItemService.CreateItemAsync(_model);
            Snackbar.Add("تم إضافة الصنف بنجاح", Severity.Success);
            Navigation.NavigateTo("/items");
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

    private void Cancel() => Navigation.NavigateTo("/items");
}