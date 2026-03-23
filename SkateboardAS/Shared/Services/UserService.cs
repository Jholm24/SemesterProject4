using Shared.Identity;
using Shared.Repositories;

namespace Shared.Services;

public class UserService
{
    private readonly UserRepository _repo;

    public UserService(UserRepository repo) => _repo = repo;

    public Task<IEnumerable<AppUser>> GetAllAsync() => _repo.GetAllAsync();
    public Task<AppUser?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);
    public Task AssignToLineAsync(string userId, string lineId) => _repo.AssignToLineAsync(userId, lineId);
    public Task RemoveFromLineAsync(string userId, string lineId) => _repo.RemoveFromLineAsync(userId, lineId);
}
