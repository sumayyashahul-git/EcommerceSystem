using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Domain.Models;

/// <summary>
/// The shopping cart — stored entirely in Redis as JSON
/// Key in Redis: "basket:{UserId}"
/// </summary>
public class BasketCart
{
    // Redis key = "basket:{UserId}"
    public string UserId { get; set; } = null!;

    // List of items in cart
    public List<BasketItem> Items { get; set; } = new();

    // Total price of ALL items combined
    // Calculated automatically from items
    public decimal TotalAmount => Items.Sum(i => i.TotalPrice);

    // When was cart last updated?
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Constructor
    public BasketCart(string userId)
    {
        UserId = userId;
    }

    // Parameterless constructor for JSON deserialization
    public BasketCart() { }

    /// <summary>
    /// Add item to cart or increase quantity if already exists
    /// </summary>
    public void AddItem(BasketItem newItem)
    {
        // Check if product already in cart
        var existingItem = Items
            .FirstOrDefault(i => i.ProductId == newItem.ProductId);

        if (existingItem is not null)
        {
            // Product exists → just increase quantity
            existingItem.Quantity += newItem.Quantity;
        }
        else
        {
            // New product → add to cart
            Items.Add(newItem);
        }

        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// Remove item from cart completely
    /// </summary>
    public void RemoveItem(Guid productId)
    {
        Items.RemoveAll(i => i.ProductId == productId);
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// Update quantity of existing item
    /// </summary>
    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null)
        {
            if (quantity <= 0)
                Items.Remove(item);
            else
                item.Quantity = quantity;

            LastUpdated = DateTime.UtcNow;
        }
    }
}
