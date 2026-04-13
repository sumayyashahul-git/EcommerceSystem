using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;

namespace Inventory.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _context;

    public InventoryRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryItem?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<IReadOnlyList<InventoryItem>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => await _context.InventoryItems.ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.InventoryItems
            .AnyAsync(i => i.Id == id, cancellationToken);

    public async Task AddAsync(
        InventoryItem entity,
        CancellationToken cancellationToken = default)
        => await _context.InventoryItems.AddAsync(entity, cancellationToken);

    public void Update(InventoryItem entity)
        => _context.InventoryItems.Update(entity);

    public void Delete(InventoryItem entity)
        => _context.InventoryItems.Remove(entity);

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task<InventoryItem?> GetByProductIdAsync(
        Guid productId, CancellationToken cancellationToken = default)
        => await _context.InventoryItems
            .FirstOrDefaultAsync(
                i => i.ProductId == productId, cancellationToken);

    public async Task<IReadOnlyList<InventoryItem>> GetByProductIdsAsync(
        List<Guid> productIds, CancellationToken cancellationToken = default)
        => await _context.InventoryItems
            .Where(i => productIds.Contains(i.ProductId))
            .ToListAsync(cancellationToken);
}