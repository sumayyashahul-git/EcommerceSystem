using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Exceptions;

/// <summary>
/// Thrown when a requested resource does not exist.
/// Maps to HTTP 404 Not Found.
/// 
/// Usage examples:
///   throw new NotFoundException("Product", productId);
///   → Message: "Product with Id '123' was not found"
///   
///   throw new NotFoundException("Order", orderId);
///   → Message: "Order with Id '456' was not found"
/// </summary>
public class NotFoundException : AppException
{
    // Constructor takes entity name and its Id
    // Automatically builds a meaningful message
    public NotFoundException(string entityName, object entityId)
        : base($"{entityName} with Id '{entityId}' was not found.", 404)
    {
    }

    // Overload for custom messages
    public NotFoundException(string message)
        : base(message, 404)
    {
    }
}
