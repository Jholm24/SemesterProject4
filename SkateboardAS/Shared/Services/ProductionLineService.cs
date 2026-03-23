using Shared.Entities;
using Shared.Repositories;

namespace Shared.Services;

public class ProductionLineService
{
    private readonly ProductionLineRepository _repo;

    public ProductionLineService(ProductionLineRepository repo) => _repo = repo;

    public Task<IEnumerable<ProductionLine>> GetAllAsync() => _repo.GetAllAsync();
    public Task<ProductionLine?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);
    public Task CreateAsync(ProductionLine line) => _repo.AddAsync(line);
    public Task UpdateAsync(ProductionLine line) => _repo.UpdateAsync(line);
    public Task DeleteAsync(string id) => _repo.DeleteAsync(id);
}
