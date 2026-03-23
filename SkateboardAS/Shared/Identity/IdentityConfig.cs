namespace Shared.Identity;

public class IdentityConfig
{
    public string JwtSecret { get; set; } = string.Empty;
    public string JwtIssuer { get; set; } = string.Empty;
    public string JwtAudience { get; set; } = string.Empty;
    public int JwtExpiryMinutes { get; set; } = 60;
}
