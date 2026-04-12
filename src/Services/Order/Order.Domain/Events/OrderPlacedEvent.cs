using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedKernel.Events;

namespace Order.Domain.Events;

/// <summary>
/// Raised when order is placed.
/// Published to Kafka → consumed by:
/// - Inventory Service (reserve stock)
/// - Payment Service (charge card)
/// - Notification Service (send email)
/// </summary>
public class OrderPlacedEvent : BaseEvent
{
    public Guid OrderId { get; }
    public Guid UserId { get; }
    public string OrderNumber { get; }
    public decimal TotalAmount { get; }
    public List<OrderItemEvent> Items { get; }

    public OrderPlacedEvent(
        Guid orderId,
        Guid userId,
        string orderNumber,
        decimal totalAmount,
        List<OrderItemEvent> items)
    {
        OrderId = orderId;
        UserId = userId;
        OrderNumber = orderNumber;
        TotalAmount = totalAmount;
        Items = items;
    }
}

// Nested class — item details inside the event
public class OrderItemEvent
{
    public Guid ProductId { get; }
    public string ProductName { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; }

    public OrderItemEvent(
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}