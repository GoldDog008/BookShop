using System;
using System.Collections.Generic;

namespace BookShop.BL.Model;

public partial class Residence
{
    public int Id { get; set; }

    public string? Region { get; set; }

    public string? City { get; set; }

    public string? Street { get; set; }

    public int? HouseNumber { get; set; }

    public int? ApartmentNumber { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
