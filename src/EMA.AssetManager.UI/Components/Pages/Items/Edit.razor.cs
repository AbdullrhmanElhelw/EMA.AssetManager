using EMA.AssetManager.Services.Dtos.Categories;
using EMA.AssetManager.Services.Dtos.Items;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages.Items;

public partial class Edit : ComponentBase
{
    [Inject] private IItemService ItemService { get; set; } = default!;
    [Inject] private ICategoryService CategoryService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    [Parameter] public Guid Id { get; set; } // الـ Id جاي من الرابط

    private UpdateItemDto _model = new();
    private MudForm _form = default!;
    private bool _processing = false;
    private bool _isLoading = true;

    private List<CategoryDto> _categories = new();

    // متغير وسيط عشان الـ Dropdown يتعامل مع Guid?
    private Guid? _selectedCategoryId;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        _isLoading = true;
        try
        {
            // 1. تحميل الفئات (بشكل متتابع لتجنب خطأ DbContext)
            var cats = await CategoryService.GetCategoriesAsync();
            _categories = cats.Where(c => c.IsActive).ToList();

            // 2. تحميل بيانات الصنف الحالي
            var item = await ItemService.GetItemByIdAsync(Id);

            if (item == null)
            {
                Snackbar.Add("الصنف غير موجود", Severity.Error);
                Navigation.NavigateTo("/items");
                return;
            }

            // 3. تحويل البيانات (Mapping) من DTO العرض لـ DTO التعديل
            _model = new UpdateItemDto
            {
                Name = item.Name,
                Code = item.Code,
                Unit = item.Unit,
                Quantity = item.Quantity,
                CategoryId = item.CategoryId
            };

            // ضبط القيمة المختارة في القائمة
            _selectedCategoryId = item.CategoryId;
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

        if (_selectedCategoryId == null)
        {
            Snackbar.Add("يجب اختيار الفئة", Severity.Error);
            return;
        }

        if (!_form.IsValid) return;

        _processing = true;
        try
        {
            // تحديث الفئة المختارة
            _model.CategoryId = _selectedCategoryId.Value;

            await ItemService.UpdateItemAsync(Id, _model);
            Snackbar.Add("تم تعديل الصنف بنجاح", Severity.Success);
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