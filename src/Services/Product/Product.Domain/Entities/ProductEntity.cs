using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Product.Domain.Enums;
using SharedKernel.Domain;

namespace Product.Domain.Entities;

public class ProductEntity : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public string Category { get; private set; } = null!;
    public string? ImageUrl { get; private set; }
    public ProductStatus Status { get; private set; }
    public int StockQuantity { get; private set; }

    private ProductEntity() { }

    public static ProductEntity Create(
        string name,
        string description,
        decimal price,
        string category,
        int stockQuantity,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.");

        return new ProductEntity
        {
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Price = price,
            Category = category.Trim(),
            StockQuantity = stockQuantity,
            ImageUrl = imageUrl,
            Status = ProductStatus.Active
        };
    }

    public void Update(
        string name,
        string description,
        decimal price,
        string category,
        string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Price = price;
        Category = category.Trim();
        ImageUrl = imageUrl;
        SetUpdatedAt();
    }

    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.");

        StockQuantity = quantity;

        Status = quantity == 0
            ? ProductStatus.OutOfStock
            : ProductStatus.Active;

        SetUpdatedAt();
    }

    public void Deactivate()
    {
        Status = ProductStatus.Inactive;
        SetUpdatedAt();
    }
}