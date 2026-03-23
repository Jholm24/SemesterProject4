using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Entities;

namespace Shared.Repositories;

public class FormulaRepository
{
    private readonly AppDbContext _db;

    public FormulaRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Formula>> GetAllAsync()
        => await _db.Formulas.ToListAsync();

    public async Task<Formula?> GetByIdAsync(string id)
        => await _db.Formulas.FindAsync(id);

    public async Task AddAsync(Formula formula)
    {
        _db.Formulas.Add(formula);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Formula formula)
    {
        _db.Formulas.Update(formula);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var formula = await _db.Formulas.FindAsync(id);
        if (formula != null)
        {
            _db.Formulas.Remove(formula);
            await _db.SaveChangesAsync();
        }
    }
}
