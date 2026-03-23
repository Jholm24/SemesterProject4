using Microsoft.AspNetCore.Identity;

namespace Shared.Identity;

public class AppUser : IdentityUser
{
    public string Role { get; set; } = "Operator";
    public IList<string> AssignedProductionLineIds { get; set; } = new List<string>();
}
