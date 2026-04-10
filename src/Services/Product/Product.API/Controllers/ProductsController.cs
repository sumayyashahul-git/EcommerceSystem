using MediatR;
using Microsoft.AspNetCore.Mvc;
using Product.Application.Commands.CreateProduct;
using Product.Application.Queries.GetAllProducts;
using Product.Application.Queries.GetProductById;
using SharedKernel.Common;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? category = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAllProductsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Category = category
            }, cancellationToken);

        return Ok(ApiResponse<object>.Ok(result,
            "Products retrieved successfully."));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetProductByIdQuery(id), cancellationToken);

        return Ok(ApiResponse<object>.Ok(result,
            "Product retrieved successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            ApiResponse<object>.Ok(result, "Product created successfully."));
    }
}