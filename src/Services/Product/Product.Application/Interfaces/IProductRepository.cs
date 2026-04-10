using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Product.Domain.Entities;
using SharedKernel.Common;
using SharedKernel.Interfaces;
using Product.Domain.Entities;

namespace Product.Application.Interfaces;

public interface IProductRepository : IRepository<ProductEntity>
{
    // Get products with pagination
    Task<PagedResult<ProductEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? category = null,
        CancellationToken cancellationToken = default);

    // Get all categories
    Task<IReadOnlyList<string>> GetCategoriesAsync(
        CancellationToken cancellationToken = default);

    // Check if product name exists
    Task<bool> NameExistsAsync(
        string name,
        CancellationToken cancellationToken = default);
}