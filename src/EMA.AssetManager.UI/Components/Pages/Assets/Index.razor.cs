using EMA.AssetManager.Domain.Enums;
using EMA.AssetManager.Services.Dtos.Assets;
using EMA.AssetManager.Services.Dtos.Transactions;
using EMA.AssetManager.Services.Dtos.Warehouses;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Components.Pages.Assets
{
    public partial class AssetsComponentBase : ComponentBase
    {
        // قائمة البيانات
        public List<AssetDto> _assets = new();
        public List<WarehouseDto> _warehouses = new();
        public bool _isLoading = true;
        public MudTable<AssetDto>? _table;

        // الفلاتر
        public string _searchString = "";
        public Guid? _selectedWarehouseFilter;
        public AssetStatus? _selectedStatusFilter;

        // متغيرات دايالوج الصرف
        public bool _isIssueDialogVisible;
        public IssueAssetDto _issueModel = new();
        public string _targetAssetCode = "";
        public DateTime? _issueDate = DateTime.Now;
        public bool _isSubmittingIssue;
        public MudForm? _issueForm;

        // متغيرات دايالوج الإرجاع
        public bool _isReturnDialogVisible;
        public ReturnAssetDto _returnModel = new();
        public DateTime? _returnDate = DateTime.Now;
        public bool _isSubmittingReturn;
        public MudForm? _returnForm;

        // الخدمات
        [Inject] public IAssetService? AssetService { get; set; }
        [Inject] public IWarehouseService? WarehouseService { get; set; }
        [Inject] public ITransactionService? TransactionService { get; set; }
        [Inject] public ISnackbar? Snackbar { get; set; }
        [Inject] public NavigationManager? Navigation { get; set; }

        // دالة التحميل
        public async Task OnInitializedAsync()
        {
            await LoadData();
        }

        // تحميل البيانات
        public async Task LoadData()
        {
            if (AssetService == null || WarehouseService == null)
                return;

            _isLoading = true;
            StateHasChanged();

            try
            {
                // تحميل الأصول
                var assetsResult = await AssetService.GetAssetsAsync();
                _assets = assetsResult?.ToList() ?? new List<AssetDto>();

                // تحميل المخازن
                var warehousesResult = await WarehouseService.GetWarehousesAsync();
                _warehouses = warehousesResult?.ToList() ?? new List<WarehouseDto>();

                // إعادة ضبط الفلاتر
                ResetFilters();
            }
            catch (Exception ex)
            {
                Snackbar?.Add($"خطأ في تحميل البيانات: {ex.Message}", Severity.Error);
                _assets = new List<AssetDto>();
                _warehouses = new List<WarehouseDto>();
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        // فتح دايالوج الصرف
        public void OpenIssueDialog(AssetDto asset)
        {
            try
            {
                _issueModel = new IssueAssetDto
                {
                    AssetId = asset.Id,
                    RecipientName = "",
                    Notes = ""
                };
                _targetAssetCode = asset.SerialNumber ?? asset.ItemName;
                _issueDate = DateTime.Now;
                _isIssueDialogVisible = true;

                InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Snackbar?.Add($"خطأ في فتح نموذج الصرف: {ex.Message}", Severity.Error);
            }
        }

        // فتح دايالوج الإرجاع
        public void OpenReturnDialog(AssetDto asset)
        {
            try
            {
                _returnModel = new ReturnAssetDto
                {
                    AssetId = asset.Id,
                    NewStatus = AssetStatus.Available,
                    ReturnToWarehouseId = _warehouses.FirstOrDefault()?.Id ?? Guid.Empty,
                    Notes = ""
                };
                _targetAssetCode = asset.SerialNumber ?? asset.ItemName;
                _returnDate = DateTime.Now;
                _isReturnDialogVisible = true;

                InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Snackbar?.Add($"خطأ في فتح نموذج الإرجاع: {ex.Message}", Severity.Error);
            }
        }

        // إغلاق الدايالوجات
        public void CloseDialogs()
        {
            _isIssueDialogVisible = false;
            _isReturnDialogVisible = false;
            ResetForms();
        }

        // إعادة تعيين النماذج
        private void ResetForms()
        {
            _issueModel = new IssueAssetDto();
            _returnModel = new ReturnAssetDto();
            _issueDate = DateTime.Now;
            _returnDate = DateTime.Now;

            // إعادة تعيين حقول النموذج يدوياً
            if (_issueForm != null)
            {
                // يمكنك إضافة منطق لإعادة تعيين الحقول هنا
            }

            if (_returnForm != null)
            {
                // يمكنك إضافة منطق لإعادة تعيين الحقول هنا
            }
        }

        // تنفيذ عملية الصرف
        public async Task SubmitIssue()
        {
            if (TransactionService == null)
                return;

            try
            {
                // التحقق من صحة البيانات
                if (string.IsNullOrWhiteSpace(_issueModel.RecipientName))
                {
                    Snackbar?.Add("يرجى إدخال اسم المستلم", Severity.Warning);
                    return;
                }

                _isSubmittingIssue = true;
                StateHasChanged();

                _issueModel.Date = _issueDate ?? DateTime.Now;
                await TransactionService.IssueAssetAsync(_issueModel);

                Snackbar?.Add("✅ تم صرف العهدة بنجاح", Severity.Success);
                _isIssueDialogVisible = false;

                await LoadData();
            }
            catch (Exception ex)
            {
                Snackbar?.Add($"❌ خطأ في صرف العهدة: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isSubmittingIssue = false;
                StateHasChanged();
            }
        }

        // تنفيذ عملية الإرجاع
        public async Task SubmitReturn()
        {
            if (TransactionService == null)
                return;

            try
            {
                // التحقق من صحة البيانات
                if (_returnModel.ReturnToWarehouseId == Guid.Empty)
                {
                    Snackbar?.Add("يرجى اختيار المخزن المستلم", Severity.Warning);
                    return;
                }

                _isSubmittingReturn = true;
                StateHasChanged();

                _returnModel.Date = _returnDate ?? DateTime.Now;
                await TransactionService.ReturnAssetAsync(_returnModel);

                Snackbar?.Add("✅ تم إرجاع العهدة للمخزن بنجاح", Severity.Success);
                _isReturnDialogVisible = false;

                await LoadData();
            }
            catch (Exception ex)
            {
                Snackbar?.Add($"❌ خطأ في إرجاع العهدة: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isSubmittingReturn = false;
                StateHasChanged();
            }
        }

        // فلترة البيانات
        public bool FilterFunc(AssetDto element)
        {
            // فلترة حسب الحالة
            if (_selectedStatusFilter != null && element.Status != _selectedStatusFilter)
                return false;

            // فلترة حسب المخزن
            if (_selectedWarehouseFilter != null && element.WarehouseId != _selectedWarehouseFilter)
                return false;

            // بحث نصي
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                var searchData = $"{element.SerialNumber} {element.Barcode} {element.ItemName} {element.WarehouseName}";
                if (!searchData.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        // إعادة ضبط الفلاتر
        public void ResetFilters()
        {
            _selectedStatusFilter = null;
            _selectedWarehouseFilter = null;
            _searchString = "";

            if (_table != null)
            {
                _table.CurrentPage = 0;
            }
        }

        // التنقل
        public void NavigateToCreate()
        {
            Navigation?.NavigateTo("/assets/create");
        }

        public void NavigateToEdit(Guid id)
        {
            Navigation?.NavigateTo($"/assets/edit/{id}");
        }

        // التحقق من صحة اسم المستلم
        public IEnumerable<string> ValidateRecipientName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                yield return "اسم المستلم مطلوب";
            else if (value.Length < 3)
                yield return "يجب أن يكون الاسم 3 أحرف على الأقل";
        }

        // ألوان الحالة
        public Color GetStatusColor(AssetStatus status)
        {
            return status switch
            {
                AssetStatus.Available => Color.Success,
                AssetStatus.InUse => Color.Info,
                AssetStatus.UnderMaintenance => Color.Warning,
                AssetStatus.Damaged => Color.Error,
                AssetStatus.Retired => Color.Dark,
                _ => Color.Default
            };
        }

        // أسماء الحالة بالعربية
        public string GetStatusName(AssetStatus status)
        {
            return status switch
            {
                AssetStatus.Available => "متاح",
                AssetStatus.InUse => "مستخدم",
                AssetStatus.UnderMaintenance => "صيانة",
                AssetStatus.Damaged => "تالف",
                AssetStatus.Retired => "مستبعد",
                _ => status.ToString()
            };
        }
    }
}