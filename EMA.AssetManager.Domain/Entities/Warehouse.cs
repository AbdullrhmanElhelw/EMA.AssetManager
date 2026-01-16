using EMA.AssetManager.Domain.Entities.Common;

namespace EMA.AssetManager.Domain.Entities;

public class Warehouse : AuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? KeeperName { get; set; }
    public bool IsActive { get; set; }


    public ICollection<Asset> Assets { get; set; } = [];
}