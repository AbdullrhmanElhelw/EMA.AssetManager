using System.ComponentModel.DataAnnotations;

namespace EMA.AssetManager.Domain.Entities.Common;


public abstract class Entity<TKey> : IEntity<TKey>
{
    [Key]
    public TKey Id { get; set; }
}
