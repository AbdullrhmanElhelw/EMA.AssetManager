using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private bool _drawerOpen = true;
    private bool _isRTL = true;
    private MudTheme _currentTheme = new();

    protected override void OnInitialized()
    {
        _currentTheme = CreateModernMilitaryTheme();
    }

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private MudTheme CreateModernMilitaryTheme()
    {
        var theme = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = "#1E3A2F",          // أخضر عسكري داكن
                Secondary = "#C5A059",        // ذهبي عسكري
                Tertiary = "#2D4A3E",         // أخضر متوسط
                Background = "#F8FAF7",       // خلفية فاتحة ناعمة
                Surface = "#FFFFFF",          // أسطح بيضاء

                DrawerBackground = "#1A2B26", // خلفية داكنة للدراور
                DrawerText = "#FFFFFF",       // نص فاتح
                DrawerIcon = "#C5A059",       // أيقونات ذهبية

                AppbarBackground = "#1E3A2F", // AppBar داكن
                AppbarText = "#FFFFFF",       // نص أبيض

                TextPrimary = "#1E3A2F",      // نص رئيسي داكن
                TextSecondary = "#5D7A6F",    // نص ثانوي

                Divider = "#E0E6E3",          // فواصل ناعمة

                GrayLight = "#F5F7F6",
                GrayDefault = "#E0E6E3",
                GrayDark = "#C8D1CE"
            },

            PaletteDark = new PaletteDark()
            {
                Primary = "#2D9D78",
                Secondary = "#C5A059",
                Background = "#121A17",
                Surface = "#1A2420",
                DrawerBackground = "#15201C"
            }
        };

        // إعداد الخط
        theme.Typography.Default.FontFamily = new[] { "Cairo", "Inter", "sans-serif" };

        return theme;
    }
}