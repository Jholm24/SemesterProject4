using Microsoft.AspNetCore.Identity;
using Shared.Identity;

namespace Shared.Repositories;

public class UserRepository
{
    private readonly UserManager<AppUser> _userManager;

    public UserRepository(UserManager<AppUser> userManager) => _userManager = userManager;

    public async Task<IEnumerable<AppUser>> GetAllAsync()
        => _userManager.Users.ToList();

    public async Task<AppUser?> GetByIdAsync(string id)
        => await _userManager.FindByIdAsync(id);

    public async Task AssignToLineAsync(string userId, string lineId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && !user.AssignedProductionLineIds.Contains(lineId))
        {
            user.AssignedProductionLineIds.Add(lineId);
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task RemoveFromLineAsync(string userId, string lineId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.AssignedProductionLineIds.Remove(lineId);
            await _userManager.UpdateAsync(user);
        }
    }
}
