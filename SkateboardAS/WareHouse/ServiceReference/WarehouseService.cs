using WareHouse_ServiceReference;

namespace DefaultNamespace;

public class WarehouseService : IWarehouse
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

    public async Task<string> InsertItemAsync(int trayId, string name)
    {
    var client = new EmulatorServiceClient();
    return await client.InsertItemAsync(trayId, name);
    }



    

}
