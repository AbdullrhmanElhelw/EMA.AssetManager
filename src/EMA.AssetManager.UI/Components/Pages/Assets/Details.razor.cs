using EMA.AssetManager.Services.Dtos.Assets;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EMA.AssetManager.UI.Pages.Assets;

public partial class Details : ComponentBase
{
    [Parameter] public Guid Id { get; set; }

    [Inject] private IAssetService AssetService { get; set; } = default!;
    [Inject] private ITransactionService TransactionService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private AssetDto? _asset;
    private List<AssetTransactionDto> _history = new();
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        try
        {
            // 1. تحميل بيانات الأصل
            _asset = await AssetService.GetAssetByIdAsync(Id);

            // 2. تحميل السجل (تأكد إن الدالة دي موجودة في TransactionService)
            _history = await TransactionService.GetAssetHistoryAsync(Id);
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

    // --- دوال المساعدة للتايم لاين ---

    private Color GetColor(TransactionType type)
    {
        return type switch
        {
            TransactionType.Issue => Color.Warning,   // صرف (أصفر)
            TransactionType.Return => Color.Success,  // إرجاع (أخضر)
            _ => Color.Info
        };
    }

    private string GetIcon(TransactionType type)
    {
        return type switch
        {
            TransactionType.Issue => Icons.Material.Filled.Output,
            TransactionType.Return => Icons.Material.Filled.Input,
            _ => Icons.Material.Filled.History
        };
    }

    private string GetTitle(TransactionType type)
    {
        return type switch
        {
            TransactionType.Issue => "صرف عهدة",
            TransactionType.Return => "إرجاع للمخزن",
            _ => type.ToString()
        };
    }
}