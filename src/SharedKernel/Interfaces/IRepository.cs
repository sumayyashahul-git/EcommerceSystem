using SharedKernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Interfaces;

/// <summary>
/// Generic repository interface defining standard data operations.
/// Every service implements this for their own entities.
///
/// T must be a BaseEntity — ensures only proper entities use this
///
/// Usage:
///   IRepository<Product>   → in Product Service
///   IRepository<Order>     → in Order Service
///   IRepository<Payment>   → in Payment Service
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    // =====================
    // READ OPERATIONS
    // =====================

    // Get single entity by its Id
    // Returns null if not found — caller decides what to do
    // Example: var product = await _repo.GetByIdAsync(productId);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Get ALL entities — use carefully on large datasets!
    // For large data, use GetPagedAsync instead
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    // Check if entity exists without loading full data
    // More efficient than GetByIdAsync when you only need to check existence
    // Example: if (!await _repo.ExistsAsync(productId)) throw new NotFoundException(...)
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    // =====================
    // WRITE OPERATIONS
    // =====================

    // Add new entity to database
    // Does NOT save immediately — call SaveChangesAsync() after
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    // Update existing entity
    // Does NOT save immediately — call SaveChangesAsync() after
    void Update(T entity);

    // Delete entity
    // Does NOT save immediately — call SaveChangesAsync() after
    void Delete(T entity);

    // =====================
    // SAVE OPERATION
    // =====================

    // Commits ALL pending changes to database in ONE transaction
    // Called ONCE after all operations are done
    // Returns number of records affected
    //
    // Why separate from Add/Update/Delete?
    // Allows multiple operations in ONE transaction:
    //   await _repo.AddAsync(order);
    //   await _repo.AddAsync(orderItem1);
    //   await _repo.AddAsync(orderItem2);
    //   await _repo.SaveChangesAsync(); ← all saved together atomically
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}