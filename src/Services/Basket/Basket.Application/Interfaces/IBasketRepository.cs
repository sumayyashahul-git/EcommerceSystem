using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Basket.Domain.Models;

namespace Basket.Application.Interfaces;

/// <summary>
/// Contract for basket data operations.
/// Implemented in Infrastructure using Redis.
/// </summary>
public interface IBasketRepository
{
    // Get cart by userId
    // Returns null if cart doesn't exist
    Task<BasketCart?> GetBasketAsync(string userId);

    // Save or update cart
    // TTL = 24 hours (cart expires automatically)
    Task<BasketCart> UpdateBasketAsync(BasketCart basket);

    // Delete cart completely (on checkout or manual clear)
    Task DeleteBasketAsync(string userId);

    // Check if cart exists
    Task<bool> BasketExistsAsync(string userId);
}