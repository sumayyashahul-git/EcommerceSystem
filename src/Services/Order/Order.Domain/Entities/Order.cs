using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Order.Domain.Enums;
using Order.Domain.Events;
using SharedKernel.Domain;

namespace Order.Domain.Entities;

/// <summary>
/// Order Aggregate Root.
/// Controls ALL order-related operations.
/// Raises domain events when important things happen.
/// </summary>
public class Order : AggregateRoot
{
    // Who placed this order
    public Guid UserId { get; private set; }

    // Unique order reference (human readable)
    public string OrderNumber { get; private set; } = null!;

    // Current status
    public OrderStatus Status { get; private set; }

    // Financial
    public decimal TotalAmount { get; private set; }

    // Shipping address
    public string ShippingAddress { get; private set; } = null!;

    // Notes
    public string? Notes { get; private set; }

    // Order items — private list
    // External code uses OrderItems (read-only)
    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems
        => _orderItems.AsReadOnly();

    private Order() { }

    /// <summary>
    /// Factory method — creates a new order
    /// Also raises OrderPlacedEvent for Kafka
    /// </summary>
    public static Order Create(
        Guid userId,
        string shippingAddress,
        List<(Guid ProductId, string ProductName,
              decimal UnitPrice, int Quantity)> items,
        string? notes = null)
    {
        if (!items.Any())
            throw new ArgumentException("Order must have at least one item.");

        var order = new Order
        {
            UserId = userId,
            ShippingAddress = shippingAddress,
            Notes = notes,
            Status = OrderStatus.Pending,
            OrderNumber = GenerateOrderNumber()
        };

        // Add all items
        foreach (var item in items)
        {
            var orderItem = OrderItem.Create(
                order.Id,
                item.ProductId,
                item.ProductName,
                item.UnitPrice,
                item.Quantity);

            order._orderItems.Add(orderItem);
        }

        // Calculate total
        order.TotalAmount = order._orderItems
            .Sum(i => i.TotalPrice);

        // Raise domain event → will be published to Kafka
        order.AddDomainEvent(new OrderPlacedEvent(
            order.Id,
            order.UserId,
            order.OrderNumber,
            order.TotalAmount,
            order._orderItems.Select(i => new OrderItemEvent(
                i.ProductId,
                i.ProductName,
                i.UnitPrice,
                i.Quantity)).ToList()));

        return order;
    }

    // Called when payment is confirmed
    public void Confirm()
    {
        Status = OrderStatus.Confirmed;
        SetUpdatedAt();
    }

    // Called when order is cancelled
    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException(
                "Cannot cancel a delivered order.");

        Status = OrderStatus.Cancelled;
        Notes = reason;
        SetUpdatedAt();
    }

    // Called when order is shipped
    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException(
                "Only confirmed orders can be shipped.");

        Status = OrderStatus.Shipped;
        SetUpdatedAt();
    }

    // Generate readable order number
    // Example: ORD-20240410-A3F9
    private static string GenerateOrderNumber()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = Guid.NewGuid().ToString("N")[..4].ToUpper();
        return $"ORD-{date}-{random}";
    }
}