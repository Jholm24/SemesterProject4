using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Identity;

namespace SkateboardAS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // TODO: Implement - verify credentials, issue JWT
        await Task.CompletedTask;
        return Ok(new { token = "TODO" });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // TODO: Implement - create user, assign role
        await Task.CompletedTask;
        return Ok();
    }
}

public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Password, string Role);
