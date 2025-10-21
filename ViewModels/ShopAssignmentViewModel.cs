namespace InventoryService.ViewModels;

public class ShopAssignmentViewModel
{
    public int ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}