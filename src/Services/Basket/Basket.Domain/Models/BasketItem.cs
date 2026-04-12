using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Domain.Models;

/// <summary>
/// Represents a single item in the shopping cart
/// </summary>
public class BasketItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }

    // Calculated — total for this line item
    // Example: 2 x iPhone @ 999.99 = 1999.98
    public decimal TotalPrice => Price * Quantity;
}