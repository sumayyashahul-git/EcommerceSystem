using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Order.Domain.Entities;
using SharedKernel.Common;
using SharedKernel.Interfaces;

namespace Order.Application.Interfaces;

public interface IOrderRepository : IRepository<Order.Domain.Entities.Order>
{
    // Get all orders for a specific user
    Task<PagedResult<Order.Domain.Entities.Order>> GetUserOrdersAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    // Get order by order number (human readable)
    Task<Order.Domain.Entities.Order?> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken = default);

    // Get order with all items included
    Task<Order.Domain.Entities.Order?> GetWithItemsAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);
}