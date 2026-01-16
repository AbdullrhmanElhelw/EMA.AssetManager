using EMA.AssetManager.Services.Dtos.Categories;
// تأكد من وجود الـ DTO ده، لو مكانه مختلف عدل الـ using
using EMA.AssetManager.Services.Dtos.Categories.Create;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages.Categories;

public partial class Index : ComponentBase
{
    [Inject] private ICategoryService CategoryService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;

    private List<CategoryDto>? _categories;
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadCategories();
    }

    private async Task LoadCategories()
    {
        _isLoading = true;
        try
        {
            var result = await CategoryService.GetCategoriesAsync();
            _categories = result.ToList();
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

    private void NavigateToCreate()
    {
        Navigation.NavigateTo("/categories/create");
    }

    private void NavigateToEdit(Guid id)
    {
        Navigation.NavigateTo($"/categories/edit/{id}");
    }

    // دالة تغيير الحالة (Toggle Switch)
    private async Task ToggleStatus(CategoryDto category, bool newStatus)
    {
        try
        {
            // إعداد كائن التحديث
            // ملاحظة: تأكد ان UpdateCategoryDto معرف عندك
            var updateDto = new UpdateCategoryDto
            {
                Name = category.Name,
                IsActive = newStatus
            };

            await CategoryService.UpdateCategoryAsync(category.Id, updateDto);

            // تحديث الواجهة فوراً
            category.IsActive = newStatus;
            Snackbar.Add($"تم تغيير الحالة لـ {(newStatus ? "نشطة" : "غير نشطة")}", Severity.Success);
        }
        catch (Exception ex)
        {
            await LoadCategories(); // استرجاع الحالة الأصلية عند الخطأ
            Snackbar.Add($"فشل التحديث: {ex.Message}", Severity.Error);
        }
    }

    // دالة الحذف باستخدام MessageBox (بدون ملفات إضافية)
    private async Task ShowDeleteDialog(CategoryDto category)
    {
        if (category.ItemsCount > 0)
        {
            await DialogService.ShowMessageBox(
                "تنبيه",
                $"لا يمكن حذف الفئة '{category.Name}' لأنها تحتوي على {category.ItemsCount} عنصر.",
                yesText: "موافق");
            return;
        }

        var result = await DialogService.ShowMessageBox(
            "تأكيد الحذف",
            $"هل أنت متأكد من حذف الفئة '{category.Name}'؟",
            yesText: "حذف نهائي",
            cancelText: "إلغاء",
            options: new DialogOptions { FullWidth = true, MaxWidth = MaxWidth.ExtraSmall });

        if (result == true)
        {
            await DeleteCategory(category.Id);
        }
    }

    private async Task DeleteCategory(Guid id)
    {
        try
        {
            await CategoryService.DeleteCategoryAsync(id);
            Snackbar.Add("تم حذف الفئة بنجاح", Severity.Success);
            await LoadCategories();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
}