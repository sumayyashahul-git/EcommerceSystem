using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Payment.Application.Interfaces;
using Payment.Domain.Entities;
using Payment.Infrastructure.Persistence;
using SharedKernel.Common;

namespace Payment.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentTransaction?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.PaymentTransactions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<PaymentTransaction>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => await _context.PaymentTransactions
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.PaymentTransactions
            .AnyAsync(p => p.Id == id, cancellationToken);

    public async Task AddAsync(
        PaymentTransaction entity,
        CancellationToken cancellationToken = default)
        => await _context.PaymentTransactions
            .AddAsync(entity, cancellationToken);

    public void Update(PaymentTransaction entity)
        => _context.PaymentTransactions.Update(entity);

    public void Delete(PaymentTransaction entity)
        => _context.PaymentTransactions.Remove(entity);

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task<PaymentTransaction?> GetByOrderIdAsync(
        Guid orderId, CancellationToken cancellationToken = default)
        => await _context.PaymentTransactions
            .FirstOrDefaultAsync(
                p => p.OrderId == orderId, cancellationToken);
}
