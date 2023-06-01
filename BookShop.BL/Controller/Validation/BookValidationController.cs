using BookShop.BL.Controller.IValidationData;
using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;

namespace BookShop.BL.Controller.Validation
{
    public class BookValidationController : IBookValidationData
    {
        public bool IsAuthorIdValid(int? authorId)
        {
            if (authorId < 0)
            {
                throw new InvalidOperationException("Идентификатор автора не может быть меньше 0");
            }
            using (BookShopDBContext db = new BookShopDBContext())
            {
                var existingAuthor = db.Authors.AsNoTracking().SingleOrDefault(a => a.Id == authorId)
                    ?? throw new InvalidOperationException($"Автора с индесом {authorId} не найдено");
            }

            return true;
        }

        public bool IsCountValid(int count)
        {
            if (count < 0)
            {
                throw new InvalidOperationException("Количество книг не может быть меньше 0");
            }

            return true;
        }

        public bool IsNameValid(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Название книги не может быть null");
            }
            if (name.Length < 2)
            {
                throw new InvalidOperationException("Название книги не может быть меньше двух символов");
            }

            return true;
        }

        public bool IsPriceValid(decimal price)
        {
            if (price <= 0)
            {
                throw new InvalidOperationException("Цена не может быть равна или меньше 0");
            }

            return true;
        }
    }
}
