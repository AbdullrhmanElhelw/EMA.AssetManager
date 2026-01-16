using EMA.AssetManager.Domain.Data; // تأكد من إضافة هذا الـ namespace
using EMA.AssetManager.Domain.Entities;
using EMA.AssetManager.Services.Dtos.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMA.AssetManager.Services.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly AssertManagerDbContext _context;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        AssertManagerDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<bool> CreateUserAsync(CreateUserDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.UserName + "@ema.mil",
                FullName = dto.FullName,
                MilitaryNumber = dto.MilitaryNumber,
                Rank = dto.Rank,
                DepartmentId = dto.DepartmentId,
                EmailConfirmed = true
            };

            // 2. محاولة إنشاء المستخدم
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"فشل إنشاء المستخدم: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, dto.Role.ToString());

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new Exception($"فشل تعيين الصلاحية: {errors}");
            }

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<string>> GetAllRolesAsync()
    {
        return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _userManager.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                UserName = u.UserName,
                MilitaryNumber = u.MilitaryNumber,
                Rank = u.Rank
            }).ToListAsync();
    }
}