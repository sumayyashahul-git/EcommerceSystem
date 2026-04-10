using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Events;

/// <summary>
/// Base class for ALL domain events across ALL microservices.
///
/// What is a Domain Event?
/// Something that HAPPENED in your system — past tense, immutable.
/// Examples:
///   OrderPlaced      → order was successfully placed
///   PaymentProcessed → payment went through
///   StockReserved    → inventory reserved items
///   UserRegistered   → new user signed up
///
/// Events are:
///   ✅ Immutable — never change after creation
///   ✅ Past tense — they describe what already happened
///   ✅ Published to Kafka — other services react to them
/// </summary>
public abstract class BaseEvent
{
    // Every event gets a unique ID
    // Useful for deduplication — if same event delivered twice,
    // consumers can detect and ignore the duplicate
    public Guid EventId { get; private set; }

    // When did this event happen?
    // Always UTC for consistency across services
    public DateTime OccurredAt { get; private set; }

    // What type of event is this?
    // Example: "OrderPlaced", "PaymentProcessed"
    // Useful for logging and event routing
    public string EventType { get; private set; }

    protected BaseEvent()
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        EventType = GetType().Name; // Automatically gets class name
    }
}