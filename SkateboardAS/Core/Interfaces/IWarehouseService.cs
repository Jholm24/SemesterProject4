using Core.Models;

namespace Core.Interfaces;

public interface IWarehouseService
{
    Task<CommandResult> PickItemAsync(string trayId, CancellationToken ct = default);
    Task<CommandResult> InsertItemAsync(string trayId, string name, CancellationToken ct = default);
    Task<Inventory> GetInventoryAsync(CancellationToken ct = default);
}
