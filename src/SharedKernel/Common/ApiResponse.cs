using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Common;

/// <summary>
/// Standard API response wrapper used by ALL services.
/// Ensures every endpoint returns a consistent structure.
///
/// Success response:
/// {
///   "success": true,
///   "message": "Product retrieved successfully",
///   "data": { "id": "...", "name": "iPhone 15" }
/// }
///
/// Error response:
/// {
///   "success": false,
///   "message": "Product with Id '123' was not found.",
///   "data": null
/// }
/// </summary>
public class ApiResponse<T>
{
    // Did the operation succeed?
    public bool Success { get; private set; }

    // Human readable message
    // Success: "Order placed successfully"
    // Error:   "Product not found"
    public string Message { get; private set; }

    // The actual data returned
    // T = any type: ProductDto, OrderDto, List<ProductDto> etc.
    // null on error responses
    public T? Data { get; private set; }

    // Private constructor — force use of static factory methods below
    // This ensures Success/Failure responses are created correctly
    private ApiResponse(bool success, string message, T? data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    // Factory method for SUCCESS responses
    // Usage: return ApiResponse<ProductDto>.Ok(productDto, "Retrieved successfully");
    public static ApiResponse<T> Ok(T data, string message = "Operation completed successfully.")
    {
        return new ApiResponse<T>(true, message, data);
    }

    // Factory method for FAILURE responses
    // Usage: return ApiResponse<ProductDto>.Fail("Product not found");
    public static ApiResponse<T> Fail(string message)
    {
        return new ApiResponse<T>(false, message, default);
    }
}