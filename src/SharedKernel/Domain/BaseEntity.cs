using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Domain;

/// <summary>
/// Base class for ALL entities across ALL microservices.
/// Every entity in Product, Order, Inventory, Payment inherits from this.
/// </summary>
public abstract class BaseEntity
{
    // Every entity MUST have a unique Id
    // We use Guid instead of int because:
    // - Microservices generate IDs independently (no shared DB sequence)
    // - Guid is globally unique across ALL services and ALL databases
    // - No risk of ID collision between services
    public Guid Id { get; protected set; }

    // Audit fields - When was this record created?
    // Set ONCE when entity is first created, never changed
    public DateTime CreatedAt { get; protected set; }

    // Audit fields - When was this record last modified?
    // Updated every time the entity changes
    public DateTime? UpdatedAt { get; protected set; }

    // protected constructor ensures:
    // - Cannot create BaseEntity directly (it's abstract anyway)
    // - Only derived classes (Product, Order etc.) can call this
    // - Automatically sets Id and CreatedAt on creation
    protected BaseEntity()
    {
        Id = Guid.NewGuid();      // Generate unique ID automatically
        CreatedAt = DateTime.UtcNow; // Always use UTC in distributed systems!
    }

    // Call this method whenever the entity is modified
    // Example: product.UpdatePrice(99.99) → calls SetUpdatedAt()
    protected void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
