using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using MediatR;
using SharedKernel.Exceptions;
using BC = BCrypt.Net.BCrypt;

namespace Identity.Application.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Find user by email
        var user = await _userRepository
            .GetByEmailAsync(request.Email, cancellationToken);

        // 2. Generic error — never reveal if email exists!
        //    Security best practice
        if (user is null)
            throw new ValidationException("Invalid email or password.");

        // 3. Check account is active
        if (!user.IsActive)
            throw new ValidationException("Your account has been deactivated.");

        // 4. Verify password against stored hash
        var passwordValid = BC.Verify(request.Password, user.PasswordHash);
        if (!passwordValid)
            throw new ValidationException("Invalid email or password.");

        // 5. Generate fresh tokens
        var token = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // 6. Update refresh token
        user.SetRefreshToken(refreshToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        // 7. Return response
        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            TokenExpiry = _jwtService.GetTokenExpiry(),
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }
        };
    }
}