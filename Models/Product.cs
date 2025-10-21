using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryService.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public ICollection<Shop> Shops { get; set; } = new List<Shop>();
}
