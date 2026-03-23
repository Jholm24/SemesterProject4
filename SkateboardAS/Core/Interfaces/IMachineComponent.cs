using Core.Enums;
using Core.Models;

namespace Core.Interfaces;

public interface IMachineComponent
{
    string Id { get; }
    string Name { get; }
    MachineType Type { get; }
    MachineStatus Status { get; }
    Task<CommandResult> StartAsync(CancellationToken ct = default);
    Task<CommandResult> StopAsync(CancellationToken ct = default);
    Task<CommandResult> ResetAsync(CancellationToken ct = default);
    Task<MachineStatusModel> GetStatusAsync(CancellationToken ct = default);
}
