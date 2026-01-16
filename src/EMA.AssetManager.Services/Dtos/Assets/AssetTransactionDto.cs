namespace EMA.AssetManager.Services.Dtos.Assets;

public class AssetTransactionDto
{
    public Guid Id { get; set; }
    public TransactionType TransactionType { get; set; } // صرف، إرجاع، صيانة
    public DateTime TransactionDate { get; set; }
    public string? RecipientName { get; set; } // المستلم (لو صرف)
    public string? WarehouseName { get; set; } // المخزن (لو إرجاع)
    public string? Notes { get; set; }
}