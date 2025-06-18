using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class MenuItem
{
    public int ItemId { get; set; }

    public int CategoryId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual MenuCategory Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
