using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Application.Interfaces;
using Payment.Domain.Entities;
using SharedKernel.Events;

namespace Payment.Application.Consumers;

/// <summary>
/// Listens to OrderPlacedIntegrationEvent
/// Simulates payment processing
/// Publishes PaymentProcessedIntegrationEvent
/// </summary>
public class OrderPlacedConsumer : IConsumer<OrderPlacedIntegrationEvent>
{
    private readonly IPaymentRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(
        IPaymentRepository repository,
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
            "Processing payment for Order {OrderNumber}",
            order.OrderNumber);

        // Create payment transaction record
        var payment = PaymentTransaction.Create(
            order.OrderId,
            order.OrderNumber,
            order.UserId,
            order.TotalAmount);

        await _repository.AddAsync(payment, context.CancellationToken);
        await _repository.SaveChangesAsync(context.CancellationToken);

        // Simulate payment processing
        // In real world → call Stripe, PayPal, etc.
        var paymentSuccess = SimulatePayment(order.TotalAmount);

        if (paymentSuccess)
        {
            // Generate fake transaction reference
            var transactionRef = $"TXN-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            payment.MarkAsCompleted(transactionRef);

            _logger.LogInformation(
                "Payment successful for Order {OrderNumber}. Ref: {Ref}",
                order.OrderNumber, transactionRef);
        }
        else
        {
            payment.MarkAsFailed("Payment declined by bank.");

            _logger.LogWarning(
                "Payment failed for Order {OrderNumber}",
                order.OrderNumber);
        }

        // Save updated status
        _repository.Update(payment);
        await _repository.SaveChangesAsync(context.CancellationToken);

        // Publish result to RabbitMQ
        await _publishEndpoint.Publish(
            new PaymentProcessedIntegrationEvent
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                Success = paymentSuccess,
                TransactionReference = paymentSuccess
                    ? payment.TransactionReference
                    : null,
                FailureReason = paymentSuccess
                    ? null
                    : payment.FailureReason
            }, context.CancellationToken);
    }

    /// <summary>
    /// Simulates payment gateway response.
    /// In production → call real payment API.
    /// Returns true 90% of time (10% failure rate for testing)
    /// </summary>
    private static bool SimulatePayment(decimal amount)
    {
        // Simulate 90% success rate
        var random = new Random();
        return random.Next(1, 11) <= 9;
    }
}
