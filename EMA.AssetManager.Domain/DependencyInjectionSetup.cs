using EMA.AssetManager.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EMA.AssetManager.Domain;

public static class DependencyInjectionSetup
{
    public static IServiceCollection AddDomain(
     this IServiceCollection services,
     IConfiguration configuration)
    {
        string connectionStringValue = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionStringValue))
        {
            throw new ArgumentNullException("Connection string not found");
        }

        services.AddDbContextFactory<AssertManagerDbContext>(options =>
            options.UseSqlServer(connectionStringValue, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
            }));

        return services;
    }

}
