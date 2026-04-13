using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SharedKernel.Events;

public class OrderPlacedIntegrationEvent : BaseEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public List<OrderItemIntegrationEvent> Items { get; set; } = new();
}

public class OrderItemIntegrationEvent
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}