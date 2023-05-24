using BookShop.BL.Model;
using System;
using System.Collections.Generic;

namespace BookShop.BL.Model;

public partial class Book
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int Count { get; set; }

    public string? Description { get; set; }

    public int? AuthorId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual ICollection<SalesHistory> SalesHistories { get; set; } = new List<SalesHistory>();

    public Book(string name, decimal price, int count, string? description = null, int? authorId = null)
    {
        Name = name; 
        Price = price;
        Count = count;
        Description = description;
        AuthorId = authorId;
    }

    public override string ToString()
    {
        return Name;
    }
}
