using Microsoft.AspNetCore.Mvc;
using Shared.Entities;
using Shared.Services;

namespace SkateboardAS.Controllers;

[ApiController]
[Route("api/production-lines")]
public class ProductionLineController : ControllerBase
{
    private readonly ProductionLineService _service;

    public ProductionLineController(ProductionLineService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var line = await _service.GetByIdAsync(id);
        return line == null ? NotFound() : Ok(line);
    }

    [HttpPost]

    public async Task<IActionResult> Create([FromBody] ProductionLine line)
    {
        await _service.CreateAsync(line);
        return CreatedAtAction(nameof(GetById), new { id = line.Id }, line);
    }

    [HttpPut("{id}")]

    public async Task<IActionResult> Update(string id, [FromBody] ProductionLine line)
    {
        await _service.UpdateAsync(line);
        return NoContent();
    }

    [HttpDelete("{id}")]

    public async Task<IActionResult> Delete(string id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
