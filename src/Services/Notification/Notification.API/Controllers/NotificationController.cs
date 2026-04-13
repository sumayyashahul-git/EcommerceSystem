using Microsoft.AspNetCore.Mvc;

namespace Notification.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            service = "Notification Service",
            status = "Running ✅",
            consumers = new[]
            {
                "OrderPlacedConsumer ✅",
                "PaymentProcessedConsumer ✅",
                "StockReservedConsumer ✅"
            },
            timestamp = DateTime.UtcNow
        });
    }
}