namespace EMA.AssetManager.Services.Dtos.Warehouses;

public class UpdateWarehouseDto
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? KeeperName { get; set; }
    public bool IsActive { get; set; }
}