using System;
using System.Collections.Generic;

namespace BookShop.BL.Model;

public partial class SalesHistory
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public int UserId { get; set; }

    public int Count { get; set; }

    public decimal Price { get; set; }

    public DateTime Date { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
