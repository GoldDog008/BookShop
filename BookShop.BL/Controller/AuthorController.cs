using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookShop.BL.Controller
{
    public class AuthorController
    {
        private Author author { get; }
        private ICollection<Book> Books { get; set; } = new List<Book>();

        /// <summary>
        /// Создать нового автора
        /// </summary>
        public AuthorController(string name)
        {
            author = new Author(name);
            using (BookShopDBContext db = new BookShopDBContext())
            {
                db.Authors.Add(author);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Получить все книги определленого автора
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetAllBooksByAuthorAsync(string name)
        {
            StringBuilder allBooks = new StringBuilder();

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var aut = db.Authors.Include(b => b.Books).SingleOrDefault(a => a.Name.Contains(name))
                    ?? throw new InvalidOperationException($"Автор с именем {name} не найден.");

                var books = aut.Books;
                foreach (Book book in books)
                {
                    allBooks.Append($"{book.Name.Trim()}, {book.Price}, {book.Count}\n");
                }

                return allBooks.ToString();
            }
        }
    }
}
