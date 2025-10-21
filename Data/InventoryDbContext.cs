using InventoryService.Models;
using Microsoft.EntityFrameworkCore;


namespace InventoryService.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {

    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Shop> Shops { get; set; }
}