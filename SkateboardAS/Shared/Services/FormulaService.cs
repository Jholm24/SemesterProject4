using Shared.Entities;
using Shared.Repositories;

namespace Shared.Services;

public class FormulaService
{
    private readonly FormulaRepository _repo;

    public FormulaService(FormulaRepository repo) => _repo = repo;

    public Task<IEnumerable<Formula>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Formula?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);
    public Task CreateAsync(Formula formula) => _repo.AddAsync(formula);
    public Task UpdateAsync(Formula formula) => _repo.UpdateAsync(formula);
    public Task DeleteAsync(string id) => _repo.DeleteAsync(id);
}
