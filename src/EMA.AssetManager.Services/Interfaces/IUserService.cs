using EMA.AssetManager.Services.Dtos.Users;

public interface IUserService
{
    Task<bool> CreateUserAsync(CreateUserDto dto);
    Task<List<string>> GetAllRolesAsync();
    Task<List<UserDto>> GetAllUsersAsync();
}