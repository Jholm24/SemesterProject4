using Microsoft.AspNetCore.Mvc;
using Shared.Entities;
using Shared.Services;

namespace SkateboardAS.Controllers;

[ApiController]
[Route("api/formulas")]
public class FormulaController : ControllerBase
{
    private readonly FormulaService _service;

    public FormulaController(FormulaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var formula = await _service.GetByIdAsync(id);
        return formula == null ? NotFound() : Ok(formula);
    }

    [HttpPost]

    public async Task<IActionResult> Create([FromBody] Formula formula)
    {
        await _service.CreateAsync(formula);
        return CreatedAtAction(nameof(GetById), new { id = formula.Id }, formula);
    }

    [HttpPut("{id}")]

    public async Task<IActionResult> Update(string id, [FromBody] Formula formula)
    {
        await _service.UpdateAsync(formula);
        return NoContent();
    }

    [HttpDelete("{id}")]

    public async Task<IActionResult> Delete(string id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
