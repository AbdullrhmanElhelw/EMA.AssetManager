using EMA.AssetManager.Services.Dtos.Categories;
using EMA.AssetManager.Services.Dtos.Items;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages.Items;

public partial class Index : ComponentBase
{
    [Inject] private IItemService ItemService { get; set; } = default!;
    [Inject] private ICategoryService CategoryService { get; set; } = default!; // عشان الفلتر
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;

    private List<ItemDto>? _items;
    private List<CategoryDto> _categories = new(); // قائمة الفئات للفلتر
    private bool _isLoading = true;

    // متغيرات الفلاتر
    private string _searchString = "";
    private Guid? _selectedCategoryFilter; // فلتر الفئة
    private int _stockStatusFilter = 0; // 0=الكل، 1=متوفر، 2=نواقص

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        _isLoading = true;
        try
        {
            // الحل: نحملهم ورا بعض عشان منعملش قفلة في الداتابيز

            // 1. نحمل الأصناف الأول
            var itemsResult = await ItemService.GetItemsAsync();
            _items = itemsResult.ToList();

            // 2. وبعدين نحمل الفئات
            var categoriesResult = await CategoryService.GetCategoriesAsync();
            _categories = categoriesResult.ToList();
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

    // دالة الفلتر الذكية (بتقبل كل الشروط)
    private bool FilterFunc(ItemDto element)
    {
        // 1. شرط البحث النصي (لو مكتوب حاجة)
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            // بنجمع كل الداتا في سطر واحد عشان البحث يبقى أسهل
            var searchData = $"{element.Name} {element.Code} {element.CategoryName} {element.Unit}";

            if (!searchData.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return false; // لو مش موجود في البحث، اخفيه
        }

        // 2. شرط فلتر الفئة (لو مختار فئة)
        if (_selectedCategoryFilter != null && _selectedCategoryFilter != Guid.Empty)
        {
            if (element.CategoryId != _selectedCategoryFilter)
                return false;
        }

        // 3. شرط حالة المخزون
        if (_stockStatusFilter == 1) // متوفر فقط
        {
            if (element.Quantity <= 0) return false;
        }
        else if (_stockStatusFilter == 2) // نواقص فقط
        {
            if (element.Quantity > 0) return false;
        }

        // لو عدى من كل الشروط دي، يبقى يظهر
        return true;
    }

    private void NavigateToCreate() => Navigation.NavigateTo("/items/create");
    private void NavigateToEdit(Guid id) => Navigation.NavigateTo($"/items/edit/{id}");

    private async Task ShowDeleteDialog(ItemDto item)
    {
        var result = await DialogService.ShowMessageBox(
            "تأكيد الحذف",
            $"هل أنت متأكد من حذف الصنف '{item.Name}'؟",
            yesText: "حذف", cancelText: "إلغاء",
            options: new DialogOptions { FullWidth = true, MaxWidth = MaxWidth.ExtraSmall });

        if (result == true)
        {
            await DeleteItem(item.Id);
        }
    }

    private async Task DeleteItem(Guid id)
    {
        try
        {
            await ItemService.DeleteItemAsync(id);
            Snackbar.Add("تم الحذف بنجاح", Severity.Success);
            await LoadData();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"فشل الحذف: {ex.Message}", Severity.Error);
        }
    }
}