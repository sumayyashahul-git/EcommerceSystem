using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Order.Application.Interfaces;
using Order.Infrastructure.Persistence;
using SharedKernel.Common;

namespace Order.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Order.Domain.Entities.Order?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<Order.Domain.Entities.Order?> GetWithItemsAsync(
        Guid orderId, CancellationToken cancellationToken = default)
        => await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

    public async Task<Order.Domain.Entities.Order?> GetByOrderNumberAsync(
        string orderNumber, CancellationToken cancellationToken = default)
        => await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(
                o => o.OrderNumber == orderNumber, cancellationToken);

    public async Task<IReadOnlyList<Order.Domain.Entities.Order>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => await _context.Orders.ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.Orders
            .AnyAsync(o => o.Id == id, cancellationToken);

    public async Task AddAsync(
        Order.Domain.Entities.Order entity,
        CancellationToken cancellationToken = default)
        => await _context.Orders.AddAsync(entity, cancellationToken);

    public void Update(Order.Domain.Entities.Order entity)
        => _context.Orders.Update(entity);

    public void Delete(Order.Domain.Entities.Order entity)
        => _context.Orders.Remove(entity);

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task<PagedResult<Order.Domain.Entities.Order>> GetUserOrdersAsync(
        Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Order.Domain.Entities.Order>.Create(
            items, totalCount, pageNumber, pageSize);
    }
}