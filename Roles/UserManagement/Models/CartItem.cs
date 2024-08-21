using System;
using System.Collections.Generic;

namespace UserManagement.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public string BookName { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual Cart Cart { get; set; } = null!;
}
