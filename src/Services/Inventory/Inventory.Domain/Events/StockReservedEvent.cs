using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Events;

/// <summary>
/// Published by Inventory Service
/// Consumed by Order Service (update status)
/// </summary>
public class StockReservedEvent
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
}
