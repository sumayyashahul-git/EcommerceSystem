using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Events;

namespace Identity.Infrastructure.Persistence;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tell EF Core to IGNORE BaseEvent — it's not a database table!
        // It's only used for domain events in memory
        modelBuilder.Ignore<BaseEvent>();

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(IdentityDbContext).Assembly);
    }
}