using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SkateboardAS.Controllers;

[ApiController]
[Route("api/components")]
public class ComponentDiscoveryController : ControllerBase
{
    private readonly IEnumerable<IMachineComponent> _components;

    public ComponentDiscoveryController(IEnumerable<IMachineComponent> components)
        => _components = components;

    [HttpGet]
    public IActionResult GetAll()
    {
        var cards = _components.Select(c => new
        {
            c.Id,
            c.Name,
            Type = c.Type.ToString(),
            Status = c.Status.ToString()
        });
        return Ok(cards);
    }
}
