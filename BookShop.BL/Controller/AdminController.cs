using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;
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
        public User GetUser(int id)
        {
            IsRoleValid(id);

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var user = db.Users
                    .Include(r => r.Role)
                    .Include(r => r.Residence)
                    .Include(s => s.SalesHistories)
                    .SingleOrDefault(x => x.Id == id)
                    ?? throw new InvalidOperationException($"Пользователь с Id {id} не найден."); ;
                return user;
            }
        }
        public void ChangeUserRole(User changingUser, int newRoleId)
        {
            IsRoleValid(newRoleId);
            RoleController.ChangeUserRole(User, changingUser, newRoleId);
        }
        public string GetAllUser()
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

    }
}
