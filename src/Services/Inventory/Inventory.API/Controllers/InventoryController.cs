using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryRepository _repository;

    public InventoryController(IInventoryRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// GET /api/inventory
    /// Get all inventory items
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(items,
            "Inventory retrieved successfully."));
    }

    /// <summary>
    /// GET /api/inventory/product/{productId}
    /// Get inventory for specific product
    /// </summary>
    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetByProductId(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var item = await _repository
            .GetByProductIdAsync(productId, cancellationToken);

        if (item is null)
            return NotFound(ApiResponse<object>.Fail(
                $"Inventory not found for product {productId}"));

        return Ok(ApiResponse<object>.Ok(item,
            "Inventory item retrieved successfully."));
    }

    /// <summary>
    /// POST /api/inventory
    /// Add new inventory item (Admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddInventory(
        [FromBody] AddInventoryRequest request,
        CancellationToken cancellationToken)
    {
        var item = InventoryItem.Create(
            request.ProductId,
            request.ProductName,
            request.Quantity);

        await _repository.AddAsync(item, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(item,
            "Inventory added successfully."));
    }

    /// <summary>
    /// PUT /api/inventory/{id}/restock
    /// Add stock to existing item
    /// </summary>
    [HttpPut("{id:guid}/restock")]
    public async Task<IActionResult> Restock(
        Guid id,
        [FromBody] RestockRequest request,
        CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);
        if (item is null)
            return NotFound(ApiResponse<object>.Fail("Inventory item not found."));

        item.AddStock(request.Quantity);
        _repository.Update(item);
        await _repository.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(item, "Stock updated successfully."));
    }
}

public record AddInventoryRequest
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = null!;
    public int Quantity { get; init; }
}

public record RestockRequest
{
    public int Quantity { get; init; }
}