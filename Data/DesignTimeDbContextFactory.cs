using InventoryService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InventoryService.Data;

/// <summary>
/// This factory is used by the EF Core tools (e.g., for creating migrations) at design time.
/// It creates a new instance of the DbContext by explicitly configuring the options.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        // This is a simple way to get the configuration. For more complex scenarios,
        // you might need to read from appsettings.json directly.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<InventoryDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        builder.UseSqlServer(connectionString);

        return new InventoryDbContext(builder.Options);
    }
}