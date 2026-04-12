using System.Threading;
using System.Threading.Tasks;

namespace Order.Application.Interfaces;

public interface IMessagePublisher
{
    // No topic parameter needed!
    // MassTransit handles routing automatically
    Task PublishAsync<T>(T message,
        CancellationToken cancellationToken = default)
        where T : class;
}