using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedKernel.Domain;

namespace Inventory.Domain.Entities;

public class InventoryItem : AggregateRoot
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public int AvailableQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }

    // Total physical stock = available + reserved
    public int TotalQuantity => AvailableQuantity + ReservedQuantity;

    private InventoryItem() { }

    public static InventoryItem Create(
        Guid productId,
        string productName,
        int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.");

        return new InventoryItem
        {
            ProductId = productId,
            ProductName = productName,
            AvailableQuantity = quantity,
            ReservedQuantity = 0
        };
    }

    /// <summary>
    /// Reserve stock for an order
    /// Moves quantity from Available → Reserved
    /// </summary>
    public bool TryReserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        // Not enough stock
        if (AvailableQuantity < quantity)
            return false;

        AvailableQuantity -= quantity;
        ReservedQuantity += quantity;
        SetUpdatedAt();
        return true;
    }

    /// <summary>
    /// Release reserved stock back to available
    /// Called when order is cancelled
    /// </summary>
    public void Release(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (ReservedQuantity < quantity)
            throw new InvalidOperationException("Cannot release more than reserved.");

        ReservedQuantity -= quantity;
        AvailableQuantity += quantity;
        SetUpdatedAt();
    }

    /// <summary>
    /// Confirm reservation — stock is shipped
    /// Removes from Reserved permanently
    /// </summary>
    public void ConfirmReservation(int quantity)
    {
        if (ReservedQuantity < quantity)
            throw new InvalidOperationException("Cannot confirm more than reserved.");

        ReservedQuantity -= quantity;
        SetUpdatedAt();
    }

    /// <summary>
    /// Add new stock (restocking)
    /// </summary>
    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        AvailableQuantity += quantity;
        SetUpdatedAt();
    }
}
