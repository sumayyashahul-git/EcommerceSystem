using Identity.Application.Commands.Login;
using Identity.Application.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common;

namespace Identity.API.Controllers;

/// <summary>
/// Handles authentication endpoints.
/// Controller is THIN — no business logic here!
/// Just receives request → sends to MediatR → returns response
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user account.
    /// POST /api/identity/register
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<object>.Ok(result, "Registration successful."));
    }

    /// <summary>
    /// Login with email and password.
    /// POST /api/identity/login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<object>.Ok(result, "Login successful."));
    }
}
