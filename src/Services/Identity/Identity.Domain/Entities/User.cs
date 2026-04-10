using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Identity.Domain.Enums;
using SharedKernel.Domain;

namespace Identity.Domain.Entities;

/// <summary>
/// User aggregate root — the main entity for Identity Service.
/// Inherits from AggregateRoot because:
/// - It raises domain events (UserRegistered, UserLoggedIn)
/// - It is the entry point for all user-related changes
/// </summary>
public class User : AggregateRoot
{
    // =====================
    // PROPERTIES
    // =====================

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    public UserRole Role { get; private set; }

    // Is this account active?
    // Admins can deactivate users without deleting them
    public bool IsActive { get; private set; }

    // Refresh token for JWT renewal
    // When JWT expires, client sends this to get a new JWT
    // Without needing to login again
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiry { get; private set; }

    // Full name — computed from first + last
    // No need to store separately
    public string FullName => $"{FirstName} {LastName}";

    // =====================
    // CONSTRUCTOR
    // =====================

    // Private — force use of factory method below
    private User() { }

    // =====================
    // FACTORY METHOD
    // =====================

    /// <summary>
    /// The ONLY way to create a new User.
    /// Factory method pattern — validates and creates in one place.
    /// </summary>
    public static User Create(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        UserRole role = UserRole.Customer)
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        // Create the user
        var user = new User
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLowerInvariant(), // Always store email lowercase!
            PasswordHash = passwordHash,
            Role = role,
            IsActive = true  // Active by default on registration
        };

        return user;
    }

    // =====================
    // BEHAVIOUR METHODS
    // =====================

    /// <summary>
    /// Update refresh token after successful login.
    /// Refresh token expires in 7 days.
    /// </summary>
    public void SetRefreshToken(string refreshToken)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        SetUpdatedAt();
    }

    /// <summary>
    /// Deactivate user account.
    /// Soft delete — data is preserved but user cannot login.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    /// <summary>
    /// Reactivate a previously deactivated account.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    /// <summary>
    /// Clear refresh token on logout.
    /// </summary>
    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiry = null;
        SetUpdatedAt();
    }
}