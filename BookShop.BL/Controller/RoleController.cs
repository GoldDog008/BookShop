using BookShop.BL.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.BL.Controller
{
    class RoleController
    {
        /// <summary>
        /// Сменить роль пользователя
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="changingUser"></param>
        /// <param name="newRole"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void ChangeUserRole(User admin, User changingUser, int newRoleId)
        {
            #region Проверки
            if (admin == null)
            {
                throw new ArgumentNullException("Админ не может быть null");
            }
            if (changingUser == null)
            {
                throw new ArgumentNullException("Пользователь не может быть null");
            }
            #endregion

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var existingRole = db.Roles.SingleOrDefault(r => r.Id == newRoleId);
                if (existingRole == null)
                {
                    throw new InvalidOperationException("Роль не существует");
                }

                var user = db.Users.SingleOrDefault(u => u.Id == changingUser.Id)
                    ?? throw new InvalidOperationException("Пользователь не найден");

                user.RoleId = existingRole.Id;
                db.SaveChanges();
            }

        }
    }
}
