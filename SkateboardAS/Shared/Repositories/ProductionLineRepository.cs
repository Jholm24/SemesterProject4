using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Entities;

namespace Shared.Repositories;

public class ProductionLineRepository
{
    private readonly AppDbContext _db;

    public ProductionLineRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ProductionLine>> GetAllAsync()
        => await _db.ProductionLines.ToListAsync();

    public async Task<ProductionLine?> GetByIdAsync(string id)
        => await _db.ProductionLines.FindAsync(id);

    public async Task AddAsync(ProductionLine line)
    {
        _db.ProductionLines.Add(line);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductionLine line)
    {
        _db.ProductionLines.Update(line);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var line = await _db.ProductionLines.FindAsync(id);
        if (line != null)
        {
            _db.ProductionLines.Remove(line);
            await _db.SaveChangesAsync();
        }
    }
}
