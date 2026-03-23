using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace SkateboardAS.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Manager")]
public class UserController : ControllerBase
{
    private readonly UserService _service;

    public UserController(UserService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost("{userId}/assign/{lineId}")]
    public async Task<IActionResult> Assign(string userId, string lineId)
    {
        await _service.AssignToLineAsync(userId, lineId);
        return NoContent();
    }

    [HttpDelete("{userId}/assign/{lineId}")]
    public async Task<IActionResult> Unassign(string userId, string lineId)
    {
        await _service.RemoveFromLineAsync(userId, lineId);
        return NoContent();
    }
}
