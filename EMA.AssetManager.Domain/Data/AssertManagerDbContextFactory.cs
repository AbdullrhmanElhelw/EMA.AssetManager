using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EMA.AssetManager.Domain.Data
{
    public class AssertManagerDbContextFactory : IDesignTimeDbContextFactory<AssertManagerDbContext>
    {
        public AssertManagerDbContext CreateDbContext(string[] args)
        {
            // 1. Determine the UI project folder where appsettings.json is
            string startupProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "EMA.AssetManager.UI");

            if (!Directory.Exists(startupProjectPath))
            {
                throw new DirectoryNotFoundException($"Startup project path not found: {startupProjectPath}");
            }

            // 2. Build configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(startupProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 3. Get connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }

            // 4. Build DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<AssertManagerDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
            });

            // 5. Create and return DbContext
            return new AssertManagerDbContext(optionsBuilder.Options);
        }
    }
}
