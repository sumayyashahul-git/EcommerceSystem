using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Product.Application.Interfaces;
using Product.Domain.Entities;
using Product.Infrastructure.Persistence;
using SharedKernel.Common;

namespace Product.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<ProductEntity?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ProductEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => await _context.Products.ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.Products
            .AnyAsync(p => p.Id == id, cancellationToken);

    public async Task AddAsync(
        ProductEntity entity, CancellationToken cancellationToken = default)
        => await _context.Products.AddAsync(entity, cancellationToken);

    public void Update(ProductEntity entity)
        => _context.Products.Update(entity);

    public void Delete(ProductEntity entity)
        => _context.Products.Remove(entity);

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task<PagedResult<ProductEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? category = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(p =>
                p.Name.Contains(searchTerm) ||
                p.Description.Contains(searchTerm));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<ProductEntity>.Create(
            items, totalCount, pageNumber, pageSize);
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
        => await _context.Products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);

    public async Task<bool> NameExistsAsync(
        string name, CancellationToken cancellationToken = default)
        => await _context.Products
            .AnyAsync(p => p.Name == name.Trim(), cancellationToken);
}