using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Commands.PlaceOrder;
using Order.Application.Queries.GetOrderById;
using SharedKernel.Common;

namespace Order.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// POST /api/orders
    /// Place a new order
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PlaceOrder(
        [FromBody] PlaceOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return Accepted(ApiResponse<object>.Ok(
            result,
            "Order placed successfully. Processing in background."));
    }

    /// <summary>
    /// GET /api/orders/{id}
    /// Get order by Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetOrderByIdQuery(id), cancellationToken);

        return Ok(ApiResponse<object>.Ok(
            result, "Order retrieved successfully."));
    }
}