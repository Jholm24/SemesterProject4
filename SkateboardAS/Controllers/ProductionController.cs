using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SkateboardAS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionController : ControllerBase
{
    private readonly IProductionOrchestrator _orchestrator;

    public ProductionController(IProductionOrchestrator orchestrator)
        => _orchestrator = orchestrator;

    [HttpPost("start")]
    [Authorize(Roles = "Operator,Manager")]
    public async Task<IActionResult> Start([FromBody] ProductionStartRequest request)
    {
        var status = await _orchestrator.RunProductionCycleAsync(request.TrayId, request.ProcessId);
        return Ok(status);
    }

    [HttpGet("status")]
    [Authorize(Roles = "Operator,Manager")]
    public async Task<IActionResult> GetStatus()
    {
        var status = await _orchestrator.GetStatusAsync();
        return Ok(status);
    }
}

public record ProductionStartRequest(string TrayId, string ProcessId);
