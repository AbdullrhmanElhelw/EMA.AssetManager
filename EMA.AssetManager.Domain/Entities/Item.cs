using EMA.AssetManager.Domain.Entities.Common;

namespace EMA.AssetManager.Domain.Entities;

public class Item : AuditableEntity<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Asset> Assets { get; set; } = [];
}
