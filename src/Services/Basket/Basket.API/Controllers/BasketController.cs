using Basket.Application.DTOs;
using Basket.Application.Interfaces;
using Basket.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common;

namespace Basket.API.Controllers;

/// <summary>
/// No MediatR here — direct service calls for simplicity
/// Controller → Repository → Redis
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;

    public BasketController(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    /// <summary>
    /// GET /api/basket/{userId}
    /// Get user's shopping cart
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetBasket(string userId)
    {
        var basket = await _basketRepository.GetBasketAsync(userId);

        // Return empty cart if none exists
        if (basket is null)
            basket = new BasketCart(userId);

        return Ok(ApiResponse<BasketDto>.Ok(
            MapToDto(basket),
            "Basket retrieved successfully."));
    }

    /// <summary>
    /// POST /api/basket
    /// Add item to cart or update existing item
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> UpdateBasket(
        [FromBody] UpdateBasketRequest request)
    {
        // Get existing cart or create new one
        var basket = await _basketRepository
            .GetBasketAsync(request.UserId)
            ?? new BasketCart(request.UserId);

        // Add or update item
        basket.AddItem(new BasketItem
        {
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            Price = request.Price,
            Quantity = request.Quantity,
            ImageUrl = request.ImageUrl
        });

        // Save back to Redis
        var updated = await _basketRepository.UpdateBasketAsync(basket);

        return Ok(ApiResponse<BasketDto>.Ok(
            MapToDto(updated),
            "Basket updated successfully."));
    }

    /// <summary>
    /// DELETE /api/basket/{userId}
    /// Clear entire cart (called after checkout)
    /// </summary>
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteBasket(string userId)
    {
        await _basketRepository.DeleteBasketAsync(userId);

        return Ok(ApiResponse<string>.Ok(
            userId,
            "Basket cleared successfully."));
    }

    // Map domain model to DTO
    private static BasketDto MapToDto(BasketCart basket) => new()
    {
        UserId = basket.UserId,
        TotalAmount = basket.TotalAmount,
        LastUpdated = basket.LastUpdated,
        Items = basket.Items.Select(i => new BasketItemDto
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Price = i.Price,
            Quantity = i.Quantity,
            ImageUrl = i.ImageUrl,
            TotalPrice = i.TotalPrice
        }).ToList()
    };
}

// Request model for updating basket
public class UpdateBasketRequest
{
    public string UserId { get; set; } = null!;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
}