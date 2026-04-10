using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Commands.Login;

public record LoginCommand : IRequest<AuthResponseDto>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}