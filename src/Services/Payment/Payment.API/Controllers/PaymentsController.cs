using Microsoft.AspNetCore.Mvc;
using Payment.Application.Interfaces;
using SharedKernel.Common;

namespace Payment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentRepository _repository;

    public PaymentsController(IPaymentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrderId(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var payment = await _repository
            .GetByOrderIdAsync(orderId, cancellationToken);

        if (payment is null)
            return NotFound(ApiResponse<object>.Fail(
                $"Payment not found for order {orderId}"));

        return Ok(ApiResponse<object>.Ok(payment,
            "Payment retrieved successfully."));
    }
}