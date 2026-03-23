using Microsoft.AspNetCore.SignalR;

namespace SkateboardAS.Hubs;

public class ProductionHub : Hub
{
    public async Task SendStatusUpdate(string machineId, string status)
        => await Clients.All.SendAsync("ReceiveStatusUpdate", machineId, status);
}
