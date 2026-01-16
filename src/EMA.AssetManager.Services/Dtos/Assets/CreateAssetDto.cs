using EMA.AssetManager.Domain.Enums;

namespace EMA.AssetManager.Services.Dtos.Assets;

public class CreateAssetDto
{
    public string SerialNumber { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public AssetStatus Status { get; set; } = AssetStatus.Available;
    public DateTime? PurchaseDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    // لازم نختار الصنف والمخزن
    public Guid ItemId { get; set; }
    public Guid WarehouseId { get; set; }
}
