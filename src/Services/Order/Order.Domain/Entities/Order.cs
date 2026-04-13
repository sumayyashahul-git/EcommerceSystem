using Order.Domain.Enums;
using SharedKernel.Domain;
using SharedKernel.Events;

namespace Order.Domain.Entities;

public class Order : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string OrderNumber { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string ShippingAddress { get; private set; } = null!;
    public string? Notes { get; private set; }

    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems
        => _orderItems.AsReadOnly();

    private Order() { }

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

        order.TotalAmount = order._orderItems.Sum(i => i.TotalPrice);

        order.AddDomainEvent(new OrderPlacedIntegrationEvent
        {
            OrderId = order.Id,
            UserId = order.UserId,
            OrderNumber = order.OrderNumber,
            TotalAmount = order.TotalAmount,
            Items = order._orderItems.Select(i => new OrderItemIntegrationEvent
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        });

        return order;
    }

    public void Confirm()
    {
        Status = OrderStatus.Confirmed;
        SetUpdatedAt();
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException(
                "Cannot cancel a delivered order.");
        Status = OrderStatus.Cancelled;
        Notes = reason;
        SetUpdatedAt();
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException(
                "Only confirmed orders can be shipped.");
        Status = OrderStatus.Shipped;
        SetUpdatedAt();
    }

    private static string GenerateOrderNumber()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = Guid.NewGuid().ToString("N")[..4].ToUpper();
        return $"ORD-{date}-{random}";
    }
}