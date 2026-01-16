namespace EMA.AssetManager.Services.Dtos.Warehouses;

public class WarehouseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? KeeperName { get; set; }
    public bool IsActive { get; set; }
}
