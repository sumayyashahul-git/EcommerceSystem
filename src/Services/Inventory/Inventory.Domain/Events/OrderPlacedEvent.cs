using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Events;

/// <summary>
/// Published by Order Service
/// Consumed by Inventory, Payment, Notification services
/// </summary>
public class OrderPlacedEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public List<OrderItemEvent> Items { get; set; } = new();
}

public class OrderItemEvent
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}