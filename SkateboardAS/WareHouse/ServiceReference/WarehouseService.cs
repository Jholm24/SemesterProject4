using WareHouse_ServiceReference;

namespace DefaultNamespace;

public class WarehouseService
{
    public async Task<string> PickItemAsync(int trayId)
    {
        var client = new EmulatorServiceClient();
        return await client.PickItemAsync(trayId);
    }

    public async Task<string> GetInventoryAsync()
    {
        var client = new EmulatorServiceClient();
        return await client.GetInventoryAsync();
    }
}
