using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Exceptions;

/// <summary>
/// Base exception for ALL custom exceptions in our system.
/// Every service throws this or its derived classes.
/// Maps to HTTP 500 Internal Server Error by default.
/// </summary>
public class AppException : Exception
{
    // HTTP status code this exception maps to
    public int StatusCode { get; }

    public AppException(string message, int statusCode = 500)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
