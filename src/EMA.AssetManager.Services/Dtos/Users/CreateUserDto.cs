using EMA.AssetManager.Domain.Enums;

namespace EMA.AssetManager.Services.Dtos.Users;

public class CreateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string MilitaryNumber { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Guid? DepartmentId { get; set; }
}
