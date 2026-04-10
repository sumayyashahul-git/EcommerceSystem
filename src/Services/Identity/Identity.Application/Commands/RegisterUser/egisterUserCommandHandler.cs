using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using MediatR;
using SharedKernel.Exceptions;
using BC = BCrypt.Net.BCrypt;

namespace Identity.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check email doesn't already exist
        var emailExists = await _userRepository
            .EmailExistsAsync(request.Email, cancellationToken);

        if (emailExists)
            throw new ValidationException("Email is already registered.");

        // 2. Hash password — NEVER store plain text!
        var passwordHash = BC.HashPassword(request.Password);

        // 3. Create user via factory method
        var user = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            passwordHash);

        // 4. Generate tokens
        var token = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // 5. Save refresh token on user
        user.SetRefreshToken(refreshToken);

        // 6. Persist to database
        await _userRepository.AddAsync(user, cancellationToken);
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
