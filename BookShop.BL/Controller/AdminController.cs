using BookShop.BL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.BL.Controller
{
    public class AdminController : UserController
    {
        public AdminController(string email) : base(email)
        {
        }

        public string GetAllUser()
        {
            if (User.Role.Name == "Admin")
            {
                StringBuilder usersData = new StringBuilder();

                using (BookShopDBContext sb = new BookShopDBContext())
                {
                    var users = sb.Users.ToList();
                    foreach (var user in users)
                    {
                        usersData.Append(user.ToString() + '\n');
                    }
                }
                return usersData.ToString();
            }
            else
            {
                throw new InvalidOperationException("Чтобы получить список пользователей нужно обладать правами админа");
            }
        }

    }
}
