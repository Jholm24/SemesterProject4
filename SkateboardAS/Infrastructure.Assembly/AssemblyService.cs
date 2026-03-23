using System.Composition;
using Core.Attributes;
using Core.Enums;
using Core.Interfaces;
using Core.Lifecycle;
using Core.Models;
using Infrastructure.Assembly.Internal;

namespace Infrastructure.Assembly;

[Component("Assembly Service", "1.0.0")]
[Provides(typeof(IAssemblyService))]
[Provides(typeof(IMachineComponent))]
[Export(typeof(IAssemblyService))]
[Export(typeof(IMachineComponent))]
[ExportMetadata("Name", "Assembly Service")]
[ExportMetadata("Version", "1.0.0")]
[ExportMetadata("Protocol", "MQTT")]
[ExportMetadata("MachineType", "AssemblyStation")]
[ExportMetadata("Description", "UR assembly station for product assembly")]
[ExportMetadata("Icon", "cog")]
[ExportMetadata("Priority", 2)]
public class AssemblyService : IAssemblyService, IMachineComponent, IComponent
{
    private readonly MqttClientWrapper _mqttClient = new();

    public ComponentState State { get; private set; } = ComponentState.Installed;
    public string Id => "assembly-01";
    public string Name => "Assembly Station";
    public MachineType Type => MachineType.AssemblyStation;
    public MachineStatus Status { get; private set; } = MachineStatus.Offline;

    async Task IComponent.StartAsync(CancellationToken ct)
    {
        State = ComponentState.Starting;
        await _mqttClient.ConnectAsync(ct);
        Status = MachineStatus.Idle;
        State = ComponentState.Active;
    }

    async Task IComponent.StopAsync(CancellationToken ct)
    {
        State = ComponentState.Stopping;
        await _mqttClient.DisconnectAsync(ct);
        Status = MachineStatus.Offline;
        State = ComponentState.Uninstalled;
    }

    public Task<CommandResult> StartAsync(CancellationToken ct = default) => Task.FromResult(CommandResult.Ok());
    public Task<CommandResult> StopAsync(CancellationToken ct = default) => Task.FromResult(CommandResult.Ok());
    public Task<CommandResult> ResetAsync(CancellationToken ct = default) => Task.FromResult(CommandResult.Ok());
    public Task<MachineStatusModel> GetStatusAsync(CancellationToken ct = default)
        => Task.FromResult(new MachineStatusModel { Id = Id, Name = Name, State = Status.ToString() });

    public Task<CommandResult> StartOperationAsync(string processId, CancellationToken ct = default)
        => _mqttClient.PublishOperationAsync(processId, ct);

    public Task<bool> CheckHealthAsync(CancellationToken ct = default)
        => _mqttClient.CheckHealthAsync(ct);

    public Task<AssemblyStatus> GetAssemblyStatusAsync(CancellationToken ct = default)
        => _mqttClient.GetStatusAsync(ct);
}
