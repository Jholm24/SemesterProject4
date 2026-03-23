namespace Infrastructure.Warehouse.Internal;

internal class WarehouseState
{
    public int TotalSlots { get; set; }
    public int OccupiedSlots { get; set; }
    public Dictionary<string, bool> SlotOccupancy { get; set; } = new();
}
