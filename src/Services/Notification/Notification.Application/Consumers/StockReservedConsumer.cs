using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MassTransit;
using Microsoft.Extensions.Logging;
using SharedKernel.Events;

namespace Notification.Application.Consumers;

/// <summary>
/// Listens to StockReservedIntegrationEvent
/// Logs stock reservation result
/// </summary>
public class StockReservedConsumer
    : IConsumer<StockReservedIntegrationEvent>
{
    private readonly ILogger<StockReservedConsumer> _logger;

    public StockReservedConsumer(
        ILogger<StockReservedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(
        ConsumeContext<StockReservedIntegrationEvent> context)
    {
        var stock = context.Message;

        if (stock.Success)
        {
            _logger.LogInformation(
                "📦 Stock reserved successfully for Order {OrderNumber}",
                stock.OrderNumber);
        }
        else
        {
            _logger.LogWarning(
                "📧 EMAIL SENT: Stock Unavailable!" +
                "\nOrder: {OrderNumber}" +
                "\nReason: {Reason}",
                stock.OrderNumber,
                stock.FailureReason);
        }

        await Task.Delay(100);
    }
}