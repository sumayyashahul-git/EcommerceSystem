using SharedKernel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Domain;

/// <summary>
/// Base class for Aggregate Roots in Domain Driven Design.
/// Extends BaseEntity with Domain Event collection capability.
///
/// What is an Aggregate Root?
/// - The MAIN entity that controls a cluster of related entities
/// - Example: Order controls OrderItems — you never modify OrderItem directly
/// - All changes to the aggregate go through the root
/// 
/// What are Domain Events?
/// - Things that HAPPENED in the domain
/// - Example: OrderPlaced, PaymentProcessed, StockReserved
/// - Raised inside the entity, published to Kafka after DB save
/// </summary>
public abstract class AggregateRoot : BaseEntity
{
    // Private list — only THIS class can add/remove events
    // External code cannot directly manipulate the events list
    private readonly List<BaseEvent> _domainEvents = new();

    // Public read-only view of events
    // External code can READ events but not MODIFY the list
    // IReadOnlyCollection prevents Add/Remove from outside
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Call this inside your entity methods to raise an event
    // Example: inside PlaceOrder() → AddDomainEvent(new OrderPlacedEvent(...))
    protected void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    // Called AFTER events are published to Kafka
    // Clears the list so events are not published twice
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}