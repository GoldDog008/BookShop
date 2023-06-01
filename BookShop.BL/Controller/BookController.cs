using BookShop.BL.Controller.IValidationData;
using BookShop.BL.Controller.Validation;
using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;

namespace BookShop.BL.Controller
{
    public class BookController
    {
        private IBookValidationData _validationData = new BookValidationController();
        private Book Book { get; }
        private Author Author { get; }

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
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
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
            return _validationData.IsNameValid(name);
        }
        private bool IsPriceValid(decimal price)
        {
            return _validationData.IsPriceValid(price);
        }
        private bool IsCountValid(int count)
        {
            return _validationData.IsCountValid(count);
        }
        private bool IsAuthorIdValid(int? authorId)
        {
            return _validationData.IsAuthorIdValid(authorId);
        }

        public int GetId()
        {
            return Book.Id;
        }
        public string GetDescription()
        {
            return Book?.Description ?? "Нет данных";
        }
        public int GetCount()
        {
            return Book.Count;
        }
        public decimal GetPrice()
        {
            return Book.Price;
        }
        public string GetAuthor()
        {
            return Book?.Author?.Name ?? "Нет данных";
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

        public void ChangeAuthor(int authorId, int bookId)
        {
            using (BookShopDBContext db = new BookShopDBContext())
            {
                var existingAuthor = db.Authors.SingleOrDefault(a => a.Id == authorId);
                if (existingAuthor != null) 
                {
                    var book = db.Books.SingleOrDefault(b => b.Id == bookId);
                    if (book != null) 
                    {
                        book.Author = existingAuthor;
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
