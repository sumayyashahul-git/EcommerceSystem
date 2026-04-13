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
/// Listens to OrderPlacedIntegrationEvent
/// Sends "Order Received" notification to customer
/// </summary>
public class OrderPlacedConsumer : IConsumer<OrderPlacedIntegrationEvent>
{
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(ILogger<OrderPlacedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(
        ConsumeContext<OrderPlacedIntegrationEvent> context)
    {
        var order = context.Message;

        // In production → send real email via SendGrid/SMTP
        // For now → log the notification
        _logger.LogInformation(
            "📧 EMAIL SENT: Order Received!" +
            "\nTo: Customer {UserId}" +
            "\nOrder: {OrderNumber}" +
            "\nTotal: {TotalAmount:C}" +
            "\nItems: {ItemCount}",
            order.UserId,
            order.OrderNumber,
            order.TotalAmount,
            order.Items.Count);

        // Simulate email sending delay
        await Task.Delay(100);

        _logger.LogInformation(
            "✅ Order confirmation email sent for {OrderNumber}",
            order.OrderNumber);
    }
}