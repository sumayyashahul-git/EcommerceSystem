using Inventory.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedKernel.Events;

namespace Inventory.Application.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedIntegrationEvent>
{
    private readonly IInventoryRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(
        IInventoryRepository repository,
        IPublishEndpoint publishEndpoint,
        ILogger<OrderPlacedConsumer> logger)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(
        ConsumeContext<OrderPlacedIntegrationEvent> context)
    {
        var order = context.Message;

        _logger.LogInformation(
            "Processing OrderPlaced for Order {OrderNumber}",
            order.OrderNumber);

        var reservationSuccess = true;
        var failureReason = string.Empty;

        foreach (var item in order.Items)
        {
            var inventoryItem = await _repository
                .GetByProductIdAsync(item.ProductId,
                    context.CancellationToken);

            if (inventoryItem is null)
            {
                reservationSuccess = false;
                failureReason = $"Product {item.ProductName} not found.";
                break;
            }

            var reserved = inventoryItem.TryReserve(item.Quantity);

            if (!reserved)
            {
                reservationSuccess = false;
                failureReason = $"Insufficient stock for {item.ProductName}. " +
                    $"Available: {inventoryItem.AvailableQuantity}, " +
                    $"Requested: {item.Quantity}";
                break;
            }

            _repository.Update(inventoryItem);
        }

        await _repository.SaveChangesAsync(context.CancellationToken);

        await _publishEndpoint.Publish(new StockReservedIntegrationEvent
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,
            Success = reservationSuccess,
            FailureReason = reservationSuccess ? null : failureReason
        }, context.CancellationToken);

        _logger.LogInformation(
            "Stock reservation for Order {OrderNumber}: {Status}",
            order.OrderNumber,
            reservationSuccess ? "SUCCESS" : "FAILED");
    }
}