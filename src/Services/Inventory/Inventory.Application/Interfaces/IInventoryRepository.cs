using Inventory.Domain.Entities;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Application.Interfaces;

public interface IInventoryRepository : IRepository<InventoryItem>
{
    Task<InventoryItem?> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryItem>> GetByProductIdsAsync(
        List<Guid> productIds,
        CancellationToken cancellationToken = default);
}
