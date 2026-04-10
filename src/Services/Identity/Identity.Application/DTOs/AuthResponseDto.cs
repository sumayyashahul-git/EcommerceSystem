using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs;

public class AuthResponseDto
{
    // JWT token sent with every API request
    public string Token { get; set; } = null!;

    // Used to get new JWT when expired
    public string RefreshToken { get; set; } = null!;

    // Frontend uses this to know when to refresh token
    public DateTime TokenExpiry { get; set; }

    // User details for immediate display
    public UserDto User { get; set; } = null!;
}