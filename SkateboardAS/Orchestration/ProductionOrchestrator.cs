using Core.Interfaces;
using Core.Models;

namespace Orchestration;

public class ProductionOrchestrator : IProductionOrchestrator
{
    private readonly IAgvService _agv;
    private readonly IWarehouseService _warehouse;
    private readonly IAssemblyService _assembly;

    public ProductionOrchestrator(IAgvService agv, IWarehouseService warehouse, IAssemblyService assembly)
    {
        _agv = agv;
        _warehouse = warehouse;
        _assembly = assembly;
    }

    public async Task<ProductionStatus> RunProductionCycleAsync(string trayId, string processId, CancellationToken ct = default)
    {
        // 16-step production sequence
        await _warehouse.PickItemAsync(trayId, ct);                             // 1
        await _agv.LoadProgramAsync("MoveToStorageOperation", ct);              // 2
        await _agv.ExecuteProgramAsync(ct);                                     // 3
        await _agv.LoadProgramAsync("PickWarehouseOperation", ct);              // 4
        await _agv.ExecuteProgramAsync(ct);                                     // 5
        await _agv.LoadProgramAsync("MoveToAssemblyOperation", ct);             // 6
        await _agv.ExecuteProgramAsync(ct);                                     // 7
        await _agv.LoadProgramAsync("PutAssemblyOperation", ct);                // 8
        await _agv.ExecuteProgramAsync(ct);                                     // 9
        await _assembly.StartOperationAsync(processId, ct);                     // 10
        var isHealthy = await _assembly.CheckHealthAsync(ct);                   // 11
        await _agv.LoadProgramAsync("PickAssemblyOperation", ct);               // 12
        await _agv.ExecuteProgramAsync(ct);                                     // 13
        await _agv.LoadProgramAsync("MoveToStorageOperation", ct);              // 14
        await _agv.ExecuteProgramAsync(ct);                                     // 15
        var outputTrayId = isHealthy ? "accepted-tray" : "defect-tray";         // 16
        var outputName = isHealthy ? "Accepted Product" : "Defect Product";
        await _warehouse.InsertItemAsync(outputTrayId, outputName, ct);

        return new ProductionStatus { IsRunning = false, IsHealthy = isHealthy };
    }

    public Task<ProductionStatus> GetStatusAsync(CancellationToken ct = default)
        => Task.FromResult(new ProductionStatus());
}
