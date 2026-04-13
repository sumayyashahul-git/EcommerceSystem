using Payment.Domain.Entities;
using SharedKernel.Interfaces;
using SharedKernel.Common;

namespace Payment.Application.Interfaces;

public interface IPaymentRepository : IRepository<PaymentTransaction>
{
    Task<PaymentTransaction?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);
}