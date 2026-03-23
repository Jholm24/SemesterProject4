using Core.Models;

namespace Core.Interfaces;

public interface IProductionOrchestrator
{
    Task<ProductionStatus> RunProductionCycleAsync(string trayId, string processId, CancellationToken ct = default);
    Task<ProductionStatus> GetStatusAsync(CancellationToken ct = default);
}
