namespace EMA.AssetManager.Services.Dtos.Transactions;

public class IssueAssetDto
{
    public Guid AssetId { get; set; }
    public string RecipientName { get; set; } = string.Empty; // مين اللي استلم؟
    public DateTime Date { get; set; } = DateTime.Now;
    public string? Notes { get; set; }
}
