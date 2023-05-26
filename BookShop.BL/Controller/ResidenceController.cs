using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.BL.Controller
{
    public class ResidenceController
    {
        private User User { get; }
        private Residence Residence { get; set; }

        public ResidenceController(User user)
        {
            User = user;
            Residence = User.Residence;
        }
        public ResidenceController(User user, string? region = null, string? city = null, string? street = null, int? houseNumber = null, int? apartmentNumber = null)
        {
            User = user;
            Residence = new Residence(region, city, street, houseNumber, apartmentNumber);

            using (BookShopDBContext db = new BookShopDBContext())
            {
                //db.Residences.Add(Residence);

                var us = db.Users.SingleOrDefault(u => u.Id == User.Id);
                if (us != null)
                {
                    us.Residence = Residence;
                }
                else
                {
                    throw new InvalidOperationException("Пользователь не найден");
                }


                db.SaveChanges();
            }
        }
        public bool ChangeResidenceData(string? region = null, string? city = null, string? street = null, int? houseNumber = null, int? apartmentNumber = null)
        {
            bool isDataChanged = false;

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var user = db.Users.SingleOrDefault(u => u.Id == User.Id);
                if (user != null)
                {
                    if (region != null)
                    {
                        User.Residence.Region = region;
                        user.Residence.Region = region;
                        isDataChanged = true;
                    }
                    if (city != null)
                    {
                        User.Residence.City = city;
                        user.Residence.City = city;
                        isDataChanged = true;
                    }
                    if (street != null)
                    {
                        User.Residence.Street = street;
                        user.Residence.Street = street;
                        isDataChanged = true;
                    }
                    if (houseNumber != null)
                    {
                        User.Residence.HouseNumber = houseNumber;
                        user.Residence.HouseNumber = houseNumber;
                        isDataChanged = true;
                    }
                    if (apartmentNumber != null)
                    {
                        User.Residence.ApartmentNumber = apartmentNumber;
                        user.Residence.ApartmentNumber = apartmentNumber;
                        isDataChanged = true;
                    }
                    if (isDataChanged)
                    {
                        db.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
