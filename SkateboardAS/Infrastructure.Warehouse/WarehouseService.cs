using System.Composition;
using Core.Attributes;
using Core.Enums;
using Core.Interfaces;
using Core.Lifecycle;
using Core.Models;
using Infrastructure.Warehouse.Internal;

namespace Infrastructure.Warehouse;

[Component("Warehouse Service", "1.0.0")]
[Provides(typeof(IWarehouseService))]
[Provides(typeof(IMachineComponent))]
[Export(typeof(IWarehouseService))]
[Export(typeof(IMachineComponent))]
[ExportMetadata("Name", "Warehouse Service")]
[ExportMetadata("Version", "1.0.0")]
[ExportMetadata("Protocol", "SOAP")]
[ExportMetadata("MachineType", "Warehouse")]
[ExportMetadata("Description", "Effimat automated warehouse for part storage")]
[ExportMetadata("Icon", "warehouse")]
[ExportMetadata("Priority", 1)]
public class WarehouseService : IWarehouseService, IMachineComponent, IComponent
{
    private readonly WarehouseSoapClient _client = new();

    public ComponentState State { get; private set; } = ComponentState.Installed;
    public string Id => "warehouse-01";
    public string Name => "Warehouse";
    public MachineType Type => MachineType.Warehouse;
    public MachineStatus Status { get; private set; } = MachineStatus.Offline;

    async Task IComponent.StartAsync(CancellationToken ct)
    {
        State = ComponentState.Starting;
        await _client.VerifyConnectionAsync(ct);
        Status = MachineStatus.Idle;
        State = ComponentState.Active;
    }

    async Task IComponent.StopAsync(CancellationToken ct)
    {
        State = ComponentState.Stopping;
        Status = MachineStatus.Offline;
        State = ComponentState.Uninstalled;
        await Task.CompletedTask;
    }

    public Task<CommandResult> StartAsync(CancellationToken ct = default) => Task.FromResult(CommandResult.Ok());
    public Task<CommandResult> StopAsync(CancellationToken ct = default) => Task.FromResult(CommandResult.Ok());
    public Task<CommandResult> ResetAsync(CancellationToken ct = default) => Task.FromResult(CommandResult.Ok());
    public Task<MachineStatusModel> GetStatusAsync(CancellationToken ct = default)
        => Task.FromResult(new MachineStatusModel { Id = Id, Name = Name, State = Status.ToString() });

    public Task<CommandResult> PickItemAsync(string trayId, CancellationToken ct = default)
        => _client.PickItemAsync(trayId, ct);

    public Task<CommandResult> InsertItemAsync(string trayId, string name, CancellationToken ct = default)
        => _client.InsertItemAsync(trayId, name, ct);

    public Task<Inventory> GetInventoryAsync(CancellationToken ct = default)
        => _client.GetInventoryAsync(ct);
}
