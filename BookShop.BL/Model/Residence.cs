using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookShop.BL.Model;

public partial class Residence
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    /// <summary>
    /// Область.
    /// </summary>
    public string? Region { get; set; }
    /// <summary>
    /// Город\поселение
    /// </summary>
    public string? City { get; set; }
    /// <summary>
    /// Улица
    /// </summary>
    public string? Street { get; set; }
    /// <summary>
    /// Номер дома
    /// </summary>
    public int? HouseNumber { get; set; }
    /// <summary>
    /// Номер квартиры
    /// </summary>
    public int? ApartmentNumber { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public Residence(string? region, string? city, string? street, int? houseNumber, int? apartmentNumber) 
    {
        Region = region;
        City = city;
        Street = street;
        HouseNumber = houseNumber;
        ApartmentNumber = apartmentNumber;
    }
}
