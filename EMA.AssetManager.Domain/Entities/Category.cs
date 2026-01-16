using EMA.AssetManager.Domain.Entities.Common;

namespace EMA.AssetManager.Domain.Entities;

public class Category : AuditableEntity<Guid>
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public ICollection<Item> Items { get; set; } = [];
}
