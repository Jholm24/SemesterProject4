using System.Composition;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Warehouse;

[Export(typeof(IComponentUIDescriptor))]
public class WarehouseUIDescriptor : IComponentUIDescriptor
{
    public string ComponentType => "Warehouse";

    public ComponentCardModel GetCardModel() => new()
    {
        Name = "Warehouse Service",
        Icon = "warehouse",
        MachineType = "Warehouse",
        Description = "Effimat automated warehouse for part storage",
        StatusFields = new List<string> { "InventoryCount", "State" },
        Actions = new List<string> { "PickItem", "InsertItem", "GetInventory" }
    };

    public IEnumerable<string> GetAvailableActions() => new[] { "PickItem", "InsertItem", "GetInventory" };
    public IEnumerable<string> GetDisplayedStatusFields() => new[] { "InventoryCount", "State" };
}
