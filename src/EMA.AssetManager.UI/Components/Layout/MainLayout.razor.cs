using EMA.AssetManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Components.Layout;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISettingsService SettingsService { get; set; } = default!;

    private bool _drawerOpen = true;
    private bool _isRTL = true; // عشان العربي
    private MudTheme _currentTheme = new();

    // متغيرات العرض (بنحط قيم مبدئية عشان الشكل ميبقاش فاضي لحد ما الداتابيز تحمل)
    private string _companyName = "جاري التحميل...";
    private string _branchName = "";
    private string _logoPath = "/logo.png"; // اللوجو الافتراضي

    protected override async Task OnInitializedAsync()
    {
        // 1. تحميل الإعدادات
        await LoadSettingsAsync();

        // 2. الاشتراك في حدث التغيير (عشان التحديث اللحظي)
        SettingsService.OnChange += HandleSettingsChange;
    }

    private async Task LoadSettingsAsync()
    {
        var settings = await SettingsService.GetSettingsAsync();

        _companyName = settings.CompanyName;
        _branchName = settings.BranchName;

        // التحقق من اللوجو
        _logoPath = string.IsNullOrEmpty(settings.LogoPath) ? "/logo.png" : settings.LogoPath;

        // بناء الثيم بالألوان الجديدة
        _currentTheme = CreateDynamicTheme(
            settings.PrimaryColor,
            settings.SecondaryColor,
            settings.DrawerBackgroundColor,
            settings.DrawerTextColor
        );
    }

    // الدالة دي بتشتغل لما السيرفس تقول "يا جماعة فيه تغيير حصل"
    private async void HandleSettingsChange()
    {
        await LoadSettingsAsync();
        await InvokeAsync(StateHasChanged); // أجبر الصفحة تعمل Refresh
    }

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    // دالة بناء الثيم (الطريقة الآمنة 100%)
    private MudTheme CreateDynamicTheme(string primary, string secondary, string drawerBg, string drawerText)
    {
        // 1. (Defensive Coding) لو القيم جاية فاضية حط الافتراضي
        if (string.IsNullOrWhiteSpace(primary)) primary = "#1E3A2F";
        if (string.IsNullOrWhiteSpace(secondary)) secondary = "#C5A059";
        if (string.IsNullOrWhiteSpace(drawerBg)) drawerBg = "#1A2B26";
        if (string.IsNullOrWhiteSpace(drawerText)) drawerText = "#FFFFFF";

        // 2. إنشاء كائن ثيم جديد (هو بيملا القيم الافتراضية لوحده)
        var theme = new MudTheme();

        // 3. تعديل الألوان
        theme.PaletteLight = new PaletteLight()
        {
            Primary = primary,
            Secondary = secondary,
            AppbarBackground = primary,

            // تخصيص ألوان القائمة الجانبية
            DrawerBackground = drawerBg,
            DrawerText = drawerText,
            DrawerIcon = secondary, // الأيقونات تاخد اللون الثانوي للتمييز

            // ثوابت لباقي النظام
            Tertiary = "#2D4A3E",
            Background = "#F4F6F5", // لون خلفية الصفحة رمادي فاتح جداً ومريح
            Surface = "#FFFFFF",
            TextPrimary = "#0D1F18"
        };

        // 4. تعديل الخط (بدون استخدام new Default لتجنب المشاكل)
        // بنعدل على الكائن الموجود بالفعل جوه الثيم
        theme.Typography.Default.FontFamily = new[] { "Cairo", "sans-serif" };

        return theme;
    }

    public void Dispose()
    {
        // إلغاء الاشتراك لمنع تسريب الذاكرة (Memory Leak)
        SettingsService.OnChange -= HandleSettingsChange;
    }
}