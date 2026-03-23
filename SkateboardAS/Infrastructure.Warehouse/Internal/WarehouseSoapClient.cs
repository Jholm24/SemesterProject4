using Core.Models;

namespace Infrastructure.Warehouse.Internal;

internal class WarehouseSoapClient
{
    private const string Endpoint = "http://localhost:8081/Service.asmx";

    public async Task VerifyConnectionAsync(CancellationToken ct = default)
    {
        // TODO: Implement - SOAP call to verify warehouse is reachable
        await Task.CompletedTask;
    }

    public async Task<CommandResult> PickItemAsync(string trayId, CancellationToken ct = default)
    {
        // TODO: Implement - SOAP PickItem(trayId)
        await Task.CompletedTask;
        return CommandResult.Ok();
    }

    public async Task<CommandResult> InsertItemAsync(string trayId, string name, CancellationToken ct = default)
    {
        // TODO: Implement - SOAP InsertItem(trayId, name)
        await Task.CompletedTask;
        return CommandResult.Ok();
    }

    public async Task<Inventory> GetInventoryAsync(CancellationToken ct = default)
    {
        // TODO: Implement - SOAP GetInventory()
        await Task.CompletedTask;
        return new Inventory();
    }
}
