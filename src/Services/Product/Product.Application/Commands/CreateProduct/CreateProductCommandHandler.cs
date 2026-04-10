using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using Product.Application.DTOs;
using Product.Application.Interfaces;
using SharedKernel.Exceptions;

namespace Product.Application.Commands.CreateProduct;

public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;

    public CreateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Check name uniqueness
        var nameExists = await _repository.NameExistsAsync(
            request.Name, cancellationToken);

        if (nameExists)
            throw new ValidationException(
                $"Product with name '{request.Name}' already exists.");

        // Create product via factory method
        var product = Product.Domain.Entities.ProductEntity.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Category,
            request.StockQuantity,
            request.ImageUrl);

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    private static ProductDto MapToDto(
        Product.Domain.Entities.ProductEntity  product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            Status = product.Status.ToString(),
            StockQuantity = product.StockQuantity,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
}