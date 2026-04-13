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
/// Listens to PaymentProcessedIntegrationEvent
/// Sends payment confirmation or failure notification
/// </summary>
public class PaymentProcessedConsumer
    : IConsumer<PaymentProcessedIntegrationEvent>
{
    private readonly ILogger<PaymentProcessedConsumer> _logger;

    public PaymentProcessedConsumer(
        ILogger<PaymentProcessedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(
        ConsumeContext<PaymentProcessedIntegrationEvent> context)
    {
        var payment = context.Message;

        if (payment.Success)
        {
            _logger.LogInformation(
                "📧 EMAIL SENT: Payment Confirmed!" +
                "\nOrder: {OrderNumber}" +
                "\nTransaction: {TransactionRef}" +
                "\nStatus: Payment Successful ✅",
                payment.OrderNumber,
                payment.TransactionReference);
        }
        else
        {
            _logger.LogWarning(
                "📧 EMAIL SENT: Payment Failed!" +
                "\nOrder: {OrderNumber}" +
                "\nReason: {FailureReason}" +
                "\nStatus: Payment Failed ❌",
                payment.OrderNumber,
                payment.FailureReason);
        }

        await Task.Delay(100);

        _logger.LogInformation(
            "✅ Payment notification sent for {OrderNumber}",
            payment.OrderNumber);
    }
}
