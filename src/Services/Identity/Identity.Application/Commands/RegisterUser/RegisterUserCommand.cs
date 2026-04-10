using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Commands.RegisterUser;

// record = immutable + value equality
// IRequest<AuthResponseDto> = this command returns AuthResponseDto
public record RegisterUserCommand : IRequest<AuthResponseDto>
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}