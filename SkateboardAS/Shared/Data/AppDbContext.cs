using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Entities;
using Shared.Identity;

namespace Shared.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();
    public DbSet<Formula> Formulas => Set<Formula>();
    public DbSet<TaskAssignment> TaskAssignments => Set<TaskAssignment>();
}
