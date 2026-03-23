namespace Core.Models;

public class Inventory
{
    public IList<InventoryItem> Items { get; set; } = new List<InventoryItem>();
}

public class InventoryItem
{
    public string TrayId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsOccupied { get; set; }
}
