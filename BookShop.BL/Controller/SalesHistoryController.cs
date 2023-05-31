using BookShop.BL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.BL.Controller
{
    public class SalesHistoryController
    {
        private SalesHistory sale { get; }
        public SalesHistoryController(int bookId, int userId, int count, decimal price, DateTime date)
        {
            IsBookIdValid(bookId);
            IsUserIdValid(userId);
            IsDateValid(date);

            sale = new SalesHistory(bookId, userId, count, price, date);
            using(BookShopDBContext db = new BookShopDBContext()) 
            {
                db.SalesHistories.Add(sale);
                db.SaveChanges();
            }
        }

        private bool IsBookIdValid(int bookId)
        {
            using (BookShopDBContext db = new BookShopDBContext())
            {
                var existingBook = db.Books.SingleOrDefault(b => b.Id == bookId);
                if (existingBook == null) 
                {
                    throw new ArgumentNullException($"Книга с индексом {bookId} не найдена");
                }
            }
            return true;
        }
        private bool IsUserIdValid(int userId)
        {
            using (BookShopDBContext db = new BookShopDBContext())
            {
                var existingBook = db.Users.SingleOrDefault(u => u.Id == userId);
                if (existingBook == null)
                {
                    throw new ArgumentNullException($"Пользователь с индексом {userId} не найден");
                }
            }
            return true;
        }
        private bool IsDateValid(DateTime date)
        { 
            if (date > DateTime.Now)
            {
                throw new InvalidOperationException("Некорректная дата совершения заказа");
            }
            return true;
        }
    }
}
