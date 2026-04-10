using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IJwtService
{
    // Generate JWT token containing UserId, Email, Role
    string GenerateToken(User user);

    // Generate random refresh token string
    string GenerateRefreshToken();

    // Get token expiry datetime
    DateTime GetTokenExpiry();
}