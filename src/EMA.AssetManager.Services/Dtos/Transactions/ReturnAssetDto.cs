using EMA.AssetManager.Domain.Enums;

namespace EMA.AssetManager.Services.Dtos.Transactions;

public class ReturnAssetDto
{
    public Guid AssetId { get; set; }
    public Guid ReturnToWarehouseId { get; set; } // هيرجع لأنهي مخزن؟
    public AssetStatus NewStatus { get; set; }    // حالته وهو راجع (سليم/تالف)
    public DateTime Date { get; set; } = DateTime.Now;
    public string? Notes { get; set; }
}