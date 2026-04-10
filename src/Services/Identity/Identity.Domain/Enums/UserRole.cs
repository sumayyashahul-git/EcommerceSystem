using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Enums;

/// <summary>
/// Defines the roles a user can have in our system.
/// Used for authorization — what each user is allowed to do.
/// </summary>
public enum UserRole
{
    // Regular customer — can browse, add to cart, place orders
    Customer = 1,

    // Store admin — can manage products, inventory, view all orders
    Admin = 2,

    // Super admin — full system access
    SuperAdmin = 3
}
