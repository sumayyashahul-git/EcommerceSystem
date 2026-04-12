using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Application.DTOs;

public class BasketDto
{
    public string UserId { get; set; } = null!;
    public List<BasketItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public DateTime LastUpdated { get; set; }
    public int TotalItems => Items.Sum(i => i.Quantity);
}

public class BasketItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public decimal TotalPrice { get; set; }
}
