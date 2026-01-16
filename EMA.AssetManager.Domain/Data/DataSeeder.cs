using EMA.AssetManager.Domain.Entities;
using EMA.AssetManager.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EMA.AssetManager.Domain.Data;

public static class DataSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>(); // أضف <Guid>

        string[] roleNames = { "Admin", "Officer", "Viewer" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }

        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@ema.mil",
                FullName = "System Administrator",
                MilitaryNumber = "00000",
                Rank = "General",
                EmailConfirmed = true
            };

            var createPowerUser = await userManager.CreateAsync(newAdmin, "Admin@123");

            if (createPowerUser.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, nameof(UserRole.Admin));
            }
        }
    }
}