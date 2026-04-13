using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.Entities;

namespace Payment.Infrastructure.Persistence.Configurations;

public class PaymentTransactionConfiguration
    : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.FailureReason)
            .HasMaxLength(500);

        builder.Property(p => p.TransactionReference)
            .HasMaxLength(100);

        builder.HasIndex(p => p.OrderId);
        builder.HasIndex(p => p.OrderNumber);
    }
}
