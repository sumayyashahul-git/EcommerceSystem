using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Identity.Domain.Entities;
using SharedKernel.Interfaces;

namespace Identity.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    // Check if email already registered — used during registration
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    // Find user by email — used during login
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    // Find user by refresh token — used when refreshing JWT
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}