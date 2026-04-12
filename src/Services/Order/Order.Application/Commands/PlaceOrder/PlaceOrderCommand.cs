using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Commands.PlaceOrder;

public record PlaceOrderCommand : IRequest<OrderDto>
{
    public Guid UserId { get; init; }
    public string ShippingAddress { get; init; } = null!;
    public string? Notes { get; init; }
    public List<OrderItemRequest> Items { get; init; } = new();
}

public record OrderItemRequest
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = null!;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
}