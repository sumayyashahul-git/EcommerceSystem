using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;
using SharedKernel.Events;

namespace Order.Infrastructure.Persistence;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order.Domain.Entities.Order> Orders
        => Set<Order.Domain.Entities.Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignore ALL event types
        modelBuilder.Ignore<BaseEvent>();
        modelBuilder.Ignore<OrderPlacedIntegrationEvent>();

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(OrderDbContext).Assembly);
    }
}