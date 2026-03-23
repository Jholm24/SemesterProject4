using Core.Models;

namespace Core.Interfaces;

public interface IAssemblyService
{
    Task<CommandResult> StartOperationAsync(string processId, CancellationToken ct = default);
    Task<bool> CheckHealthAsync(CancellationToken ct = default);
    Task<AssemblyStatus> GetAssemblyStatusAsync(CancellationToken ct = default);
}
