using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BookShop.BL.Controller
{
    public class AdminController : UserController
    {
        public AdminController(string email) : base(email)
        {
        }

        /// <summary>
        /// Получить пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public User GetUser(int userId)
        {
            IsUserIdValid(userId);

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var user = db.Users
                    .Include(r => r.Role)
                    .Include(r => r.Residence)
                    .Include(s => s.SalesHistories)
                    .SingleOrDefault(x => x.Id == userId)
                    ?? throw new InvalidOperationException($"Пользователь с Id {userId} не найден."); ;
                return user;
            }
        }

        /// <summary>
        /// Изменить роль пользователю
        /// </summary>
        /// <param name="changingUser"></param>
        /// <param name="newRoleId"></param>
        public void ChangeUserRole(User changingUser, int newRoleId)
        {
            IsRoleValid(newRoleId);
            RoleController.ChangeUserRole(User, changingUser, newRoleId);
        }

        /// <summary>
        /// Добавить новую книгу в бд
        /// </summary>
        /// <param name="name"></param>
        /// <param name="price"></param>
        /// <param name="count"></param>
        /// <param name="description"></param>
        /// <param name="authorId"></param>
        public void AddNewBook(string name, decimal price, int count, string? description = null, int? authorId = null)
        {
            BookController book = new BookController(name, price, count, description, authorId);
        }

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task DeleteUserAsync(int userId)
        {
            using (BookShopDBContext db = new BookShopDBContext())
            {
                var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId)
                    ?? throw new InvalidOperationException($"Пользователь с Id {userId} не найден."); ;

                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Получить список всех пользователей
        /// </summary>
        /// <returns></returns>
        public string GetAllUser()
        {
            StringBuilder usersData = new StringBuilder();

            using (BookShopDBContext sb = new BookShopDBContext())
            {
                var users = sb.Users.ToList();
                foreach (var user in users)
                {
                    usersData.Append($"Id:{user.Id} {user.ToString()}\n");
                }
            }
            return usersData.ToString();
        }

        public async Task<string> GetUserSalesHistoryAsync(int userId)
        {
            StringBuilder sales = new StringBuilder();

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var user = await db.Users.Include(s => s.SalesHistories)
                                         .ThenInclude(b => b.Book)
                                         .SingleOrDefaultAsync(u => u.Id == userId)
                    ?? throw new InvalidOperationException($"Пользователь с Id {userId} не найден."); ;

                foreach (var sale in user.SalesHistories)
                {
                    sales.Append($"Книга: {sale.Book.Name.Trim()} " +
                                 $"Количество: {sale.Count} " +
                                 $"Цена: {sale.Price} " +
                                 $"Дата: {sale.Date}\n");
                }
            }

            return sales.ToString();
        }
    }
}
