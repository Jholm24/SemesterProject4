using Core.Models;

namespace Core.Interfaces;

public interface IAgvService
{
    Task<CommandResult> LoadProgramAsync(string programName, CancellationToken ct = default);
    Task<CommandResult> ExecuteProgramAsync(CancellationToken ct = default);
    Task<AgvStatus> GetAgvStatusAsync(CancellationToken ct = default);
}
