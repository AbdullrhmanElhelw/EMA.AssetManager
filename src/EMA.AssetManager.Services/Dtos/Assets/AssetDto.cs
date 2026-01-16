using EMA.AssetManager.Domain.Enums;

namespace EMA.AssetManager.Services.Dtos.Assets;

public class AssetDto
{
    public Guid Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }

    // تواريخ
    public DateTime? PurchaseDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    // بيانات الصنف المرتبط
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;

    // بيانات المخزن الموجود فيه
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
}
