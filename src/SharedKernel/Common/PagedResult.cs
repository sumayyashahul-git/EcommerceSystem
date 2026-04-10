using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Common;

/// <summary>
/// Standard pagination wrapper for list responses.
/// Used when returning large lists that need pagination.
///
/// Example response:
/// {
///   "items": [...],
///   "totalCount": 150,
///   "pageNumber": 1,
///   "pageSize": 10,
///   "totalPages": 15,
///   "hasPreviousPage": false,
///   "hasNextPage": true
/// }
/// </summary>
public class PagedResult<T>
{
    // The actual items for this page
    public IReadOnlyList<T> Items { get; private set; }

    // Total records in the database (not just this page)
    public int TotalCount { get; private set; }

    // Current page number (starts from 1)
    public int PageNumber { get; private set; }

    // How many items per page
    public int PageSize { get; private set; }

    // Calculated properties — computed from above values
    // No need to store these, derive them automatically
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    // Factory method — cleaner to use than constructor directly
    public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }
}