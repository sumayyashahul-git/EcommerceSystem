using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,      // Order created, waiting for processing
    Processing = 2,   // Stock reserved, payment initiated
    Confirmed = 3,    // Payment successful + stock reserved
    Cancelled = 4,    // Payment failed OR stock unavailable
    Shipped = 5,      // Order dispatched
    Delivered = 6     // Order received by customer
}
