namespace EMA.AssetManager.Services.Dtos.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string MilitaryNumber { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}