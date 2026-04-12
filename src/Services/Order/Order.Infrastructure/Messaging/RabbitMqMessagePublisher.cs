using MassTransit;
using Order.Application.Interfaces;

namespace Order.Infrastructure.Messaging;

/// <summary>
/// RabbitMQ implementation using MassTransit.
/// MassTransit handles:
/// - Connection management
/// - Serialization
/// - Retry logic
/// - Dead letter queues
/// </summary>
public class RabbitMqMessagePublisher : IMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public RabbitMqMessagePublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<T>(
        T message,
        CancellationToken cancellationToken = default)
        where T : class
    {
        // MassTransit handles all RabbitMQ details:
        // - Which exchange to use
        // - How to serialize
        // - Routing to consumers
        await _publishEndpoint.Publish(message, cancellationToken);
    }
}