using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Payment.Domain.Enums;
using SharedKernel.Domain;

namespace Payment.Domain.Entities;

public class PaymentTransaction : AggregateRoot
{
    public Guid OrderId { get; private set; }
    public string OrderNumber { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? FailureReason { get; private set; }
    public string? TransactionReference { get; private set; }

    private PaymentTransaction() { }

    public static PaymentTransaction Create(
        Guid orderId,
        string orderNumber,
        Guid userId,
        decimal amount)
    {
        return new PaymentTransaction
        {
            OrderId = orderId,
            OrderNumber = orderNumber,
            UserId = userId,
            Amount = amount,
            Status = PaymentStatus.Pending
        };
    }

    public void MarkAsCompleted(string transactionReference)
    {
        Status = PaymentStatus.Completed;
        TransactionReference = transactionReference;
        SetUpdatedAt();
    }

    public void MarkAsFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        SetUpdatedAt();
    }

    public void MarkAsRefunded()
    {
        Status = PaymentStatus.Refunded;
        SetUpdatedAt();
    }
}