using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Domain.Enums;

public enum ProductStatus
{
    Active = 1,     // Available for purchase
    Inactive = 2,   // Hidden from catalog
    OutOfStock = 3  // Visible but cannot purchase
}
