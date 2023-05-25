using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;

namespace BookShop.BL.Controller
{
    public class BookController
    {
        private Book Book { get; }

        /// <summary>
        /// Добавление новой книги вручную.
        /// </summary>
        /// <param name="book"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BookController(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException("Книга не может быть null");
            }

            Book = book;
            using (BookShopDBContext db = new BookShopDBContext())
            {
                db.AddAsync(book);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Добавление новой книги.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="price"></param>
        /// <param name="count"></param>
        /// <param name="description"></param>
        /// <param name="authorId"></param>
        public BookController(string name, decimal price, int count, string? description = null, int? authorId = null)
        {
            IsNameValid(name);
            IsPriceValid(price);
            IsCountValid(count);
            if (authorId != null)
            {
                IsAuthorIdValid(authorId);
            }

            Book = new Book(name, price, count, description, authorId);

            using (BookShopDBContext db = new BookShopDBContext())
            {
                db.Books.Add(Book);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Поиск книги по названию.
        /// /// </summary>
        /// <param name="name"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public BookController(string name)
        {
            IsNameValid(name);

            using (BookShopDBContext db = new BookShopDBContext())
            {
                Book = db.Books
                    .Include(a => a.Author)
                    .Include(s => s.SalesHistories)
                    .FirstOrDefault(b => b.Name.Contains(name))
                    ?? throw new InvalidOperationException($"Книга с названием {name} не найдена."); ;
            }
        }

        private bool IsNameValid(string name)
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
        private bool IsPriceValid(decimal price)
        {
            if (price <= 0)
            {
                throw new InvalidOperationException("Цена не может быть равна или меньше 0");
            }

            return true;
        }
        private bool IsCountValid(int count)
        {
            if (count < 0)
            {
                throw new InvalidOperationException("Количество книг не может быть меньше 0");
            }

            return true;
        }
        private bool IsAuthorIdValid(int? authorId)
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

        public string? GetDescription()
        {
            return Book.Description;
        }
        public int GetCount()
        {
            return Book.Count;
        }
        public decimal GetPrice()
        {
            return Book.Price;
        }
        public Author? GetAuthor()
        {
            return Book.Author;
        }
        public override string ToString()
        {
            return Book.ToString();
        }

        public static string GetAllBook()
        {
            string booksData = "";
            using (BookShopDBContext db = new BookShopDBContext())
            {
                var books = db.Books.Include(a => a.Author).ToList();
                if (books != null) 
                {
                    foreach (var book in books)
                    {
                        booksData += book.ToString() + $" {book.Price}, {book?.Author?.Name.Trim() ?? "Нет данных"}, {book.Count}" + '\n';
                    }
                }
                else { booksData = "Не найдено ни одной книги"; }
            }
            return booksData;
        }
    }
}
