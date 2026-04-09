using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SkateboardAS.Controllers;

[ApiController]
[Route("api/components")]
public class ComponentDiscoveryController : ControllerBase
{
    private readonly IEnumerable<IMachineComponent> _components;
    private readonly IAgvService? _agv;
    private readonly IWarehouseService? _warehouse;
    private readonly IAssemblyService? _assembly;

    public ComponentDiscoveryController(
        IEnumerable<IMachineComponent> components,
        IServiceProvider services)
    {
        _components = components;
        _agv       = services.GetService<IAgvService>();
        _warehouse = services.GetService<IWarehouseService>();
        _assembly  = services.GetService<IAssemblyService>();
    }

    // ── Discovery ─────────────────────────────────────────────────

    [HttpGet]
    public IActionResult GetAll()
    {
        var cards = _components.Select(c => new
        {
            c.Id,
            c.Name,
            Type   = c.Type.ToString(),
            Status = c.Status.ToString()
        });
        return Ok(cards);
    }

    // ── Shared lifecycle (all component types) ────────────────────

    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start(string id)
    {
        var c = Find(id);
        if (c == null) return NotFound();
        var r = await c.StartAsync();
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPost("{id}/stop")]
    public async Task<IActionResult> Stop(string id)
    {
        var c = Find(id);
        if (c == null) return NotFound();
        var r = await c.StopAsync();
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPost("{id}/reset")]
    public async Task<IActionResult> Reset(string id)
    {
        var c = Find(id);
        if (c == null) return NotFound();
        var r = await c.ResetAsync();
        return r.Success ? Ok(r) : BadRequest(r);
    }

    // ── AGV-specific operations ───────────────────────────────────

    [HttpPost("{id}/load-program")]
    public async Task<IActionResult> LoadProgram(string id, [FromBody] LoadProgramRequest req)
    {
        if (_agv == null) return PluginNotLoaded("AGV");
        var r = await _agv.LoadProgramAsync(req.ProgramName);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPost("{id}/execute-program")]
    public async Task<IActionResult> ExecuteProgram(string id)
    {
        if (_agv == null) return PluginNotLoaded("AGV");
        var r = await _agv.ExecuteProgramAsync();
        return r.Success ? Ok(r) : BadRequest(r);
    }

    // ── Warehouse-specific operations ─────────────────────────────

    [HttpPost("{id}/pick-item")]
    public async Task<IActionResult> PickItem(string id, [FromBody] PickItemRequest req)
    {
        if (_warehouse == null) return PluginNotLoaded("Warehouse");
        var r = await _warehouse.PickItemAsync(req.TrayId);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPost("{id}/insert-item")]
    public async Task<IActionResult> InsertItem(string id, [FromBody] InsertItemRequest req)
    {
        if (_warehouse == null) return PluginNotLoaded("Warehouse");
        var r = await _warehouse.InsertItemAsync(req.TrayId, req.Name);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    // ── Assembly-specific operations ──────────────────────────────

    [HttpPost("{id}/start-operation")]
    public async Task<IActionResult> StartOperation(string id, [FromBody] StartOperationRequest req)
    {
        if (_assembly == null) return PluginNotLoaded("Assembly");
        var r = await _assembly.StartOperationAsync(req.ProcessId);
        return r.Success ? Ok(r) : BadRequest(r);
    }

    [HttpPost("{id}/check-health")]
    public async Task<IActionResult> CheckHealth(string id)
    {
        if (_assembly == null) return PluginNotLoaded("Assembly");
        var healthy = await _assembly.CheckHealthAsync();
        return Ok(new CommandResultDto(healthy, healthy ? "Healthy" : "Unhealthy"));
    }

    // ── Helpers ───────────────────────────────────────────────────

    private IMachineComponent? Find(string id) =>
        _components.FirstOrDefault(c => c.Id == id);

    private ObjectResult PluginNotLoaded(string plugin) =>
        StatusCode(503, new { message = $"{plugin} plugin er ikke indlæst." });
}

public record LoadProgramRequest(string ProgramName);
public record PickItemRequest(string TrayId);
public record InsertItemRequest(string TrayId, string Name);
public record StartOperationRequest(string ProcessId);
public record CommandResultDto(bool Success, string Message);
