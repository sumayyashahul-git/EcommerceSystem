using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Exceptions;

/// <summary>
/// Thrown when input data fails validation rules.
/// Maps to HTTP 400 Bad Request.
///
/// Usage examples:
///   throw new ValidationException("Price must be greater than zero");
///   throw new ValidationException(validationErrors); // list of errors
/// </summary>
public class ValidationException : AppException
{
    // Stores multiple validation errors
    // Example: ["Name is required", "Price must be > 0"]
    public IReadOnlyList<string> Errors { get; }

    // Single error message
    public ValidationException(string message)
        : base(message, 400)
    {
        Errors = new List<string> { message };
    }

    // Multiple validation errors at once
    // Used with FluentValidation — validates all fields together
    public ValidationException(IEnumerable<string> errors)
        : base("One or more validation errors occurred.", 400)
    {
        Errors = errors.ToList();
    }
}