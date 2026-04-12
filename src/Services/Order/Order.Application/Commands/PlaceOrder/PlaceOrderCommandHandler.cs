using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using Order.Application.DTOs;
using Order.Application.Interfaces;
using Order.Domain.Events;

namespace Order.Application.Commands.PlaceOrder;

public class PlaceOrderCommandHandler
    : IRequestHandler<PlaceOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessagePublisher _messagePublisher;

    public PlaceOrderCommandHandler(
        IOrderRepository orderRepository,
        IMessagePublisher messagePublisher)
    {
        _orderRepository = orderRepository;
        _messagePublisher = messagePublisher;
    }

    public async Task<OrderDto> Handle(
        PlaceOrderCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1 — Validate
        if (!request.Items.Any())
            throw new SharedKernel.Exceptions.ValidationException(
                "Order must have at least one item.");

        // Step 2 — Create Order
        var order = Order.Domain.Entities.Order.Create(
            request.UserId,
            request.ShippingAddress,
            request.Items.Select(i => (
                i.ProductId,
                i.ProductName,
                i.UnitPrice,
                i.Quantity)).ToList(),
            request.Notes);

        // Step 3 — Save to SQL Server
        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        // Step 4 — Publish to RabbitMQ via MassTransit
        var orderPlacedEvent = order.DomainEvents
            .OfType<OrderPlacedEvent>()
            .FirstOrDefault();

        if (orderPlacedEvent != null)
        {
            await _messagePublisher.PublishAsync(
                orderPlacedEvent, cancellationToken);

            order.ClearDomainEvents();
        }

        return MapToDto(order);
    }

    private static OrderDto MapToDto(
        Order.Domain.Entities.Order order) => new()
        {
            Id = order.Id,
            UserId = order.UserId,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            ShippingAddress = order.ShippingAddress,
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.OrderItems.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
}