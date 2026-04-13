using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infrastructure.Persistence.Configurations;

public class OrderConfiguration
    : IEntityTypeConfiguration<Order.Domain.Entities.Order>
{
    public void Configure(
        EntityTypeBuilder<Order.Domain.Entities.Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);

        // ← This line is critical!
        builder.Ignore(o => o.DomainEvents);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.ShippingAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.UserId);
    }
}