using InventoryService.Models;

namespace InventoryService.ViewModels;

public class ProductViewModel
{
    public Product Product { get; set; } = new();
    public List<ShopAssignmentViewModel> AvailableShops { get; set; } = new();
}