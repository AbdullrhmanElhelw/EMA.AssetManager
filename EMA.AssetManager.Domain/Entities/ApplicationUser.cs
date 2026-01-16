using Microsoft.AspNetCore.Identity;

namespace EMA.AssetManager.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string MilitaryNumber { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;

    public Guid? DepartmentId { get; set; }
    public virtual Department? Department { get; set; }
}
