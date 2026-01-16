using EMA.AssetManager.Domain.Enums;

namespace EMA.AssetManager.Services.Dtos.Assets;

public class UpdateAssetDto
{
    public string SerialNumber { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Guid WarehouseId { get; set; } // ممكن ننقل العهدة لمخزن تاني
    // غالباً مش هنسمح بتغيير الصنف (ItemId) بعد الإنشاء، إلا لو دي حاجة مطلوبة
}