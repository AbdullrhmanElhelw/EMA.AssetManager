using EMA.AssetManager.Domain.Entities.Common;

namespace EMA.AssetManager.Domain.Entities;

public sealed class Department : AuditableEntity<Guid>
{
    public string Name { get; set; }

    public ICollection<ApplicationUser> Users = [];
}
