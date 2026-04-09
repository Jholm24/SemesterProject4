using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace SkateboardAS.Services;

public class DevAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly RoleState _roleState;

    public DevAuthStateProvider(RoleState roleState)
    {
        _roleState = roleState;
        _roleState.OnChange += Notify;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Role, _roleState.CurrentRole) },
            "dev");
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    private void Notify() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public void Dispose() => _roleState.OnChange -= Notify;
}
