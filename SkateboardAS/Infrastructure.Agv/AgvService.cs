using System.Composition;
using Core.Attributes;
using Core.Enums;
using Core.Interfaces;
using Core.Lifecycle;
using Core.Models;
using Infrastructure.Agv.Internal;

namespace Infrastructure.Agv;

[Component("AGV Service", "1.0.0")]
[Provides(typeof(IAgvService))]
[Provides(typeof(IMachineComponent))]
[Export(typeof(IAgvService))]
[Export(typeof(IMachineComponent))]
[ExportMetadata("Name", "AGV Service")]
[ExportMetadata("Version", "1.0.0")]
[ExportMetadata("Protocol", "REST")]
[ExportMetadata("MachineType", "AGV")]
[ExportMetadata("Description", "Automated Guided Vehicle for part transport")]
[ExportMetadata("Icon", "truck")]
[ExportMetadata("Priority", 0)]
public class AgvService : IAgvService, IMachineComponent, IComponent
{
    private readonly AgvHttpClient _client = new();

    public ComponentState State { get; private set; } = ComponentState.Installed;
    public string Id => "agv-01";
    public string Name => "AGV";
    public MachineType Type => MachineType.AGV;
    public MachineStatus Status { get; private set; } = MachineStatus.Offline;

    // IComponent lifecycle
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

    // IMachineComponent
    public Task<CommandResult> StartAsync(CancellationToken ct = default)
        => Task.FromResult(CommandResult.Ok());

    public Task<CommandResult> StopAsync(CancellationToken ct = default)
        => Task.FromResult(CommandResult.Ok());

    public Task<CommandResult> ResetAsync(CancellationToken ct = default)
        => Task.FromResult(CommandResult.Ok());

    public Task<MachineStatusModel> GetStatusAsync(CancellationToken ct = default)
        => Task.FromResult(new MachineStatusModel { Id = Id, Name = Name, State = Status.ToString() });

    // IAgvService
    public Task<CommandResult> LoadProgramAsync(string programName, CancellationToken ct = default)
        => _client.LoadProgramAsync(programName, ct);

    public Task<CommandResult> ExecuteProgramAsync(CancellationToken ct = default)
        => _client.ExecuteProgramAsync(ct);

    public Task<AgvStatus> GetAgvStatusAsync(CancellationToken ct = default)
        => _client.GetStatusAsync(ct);
}
