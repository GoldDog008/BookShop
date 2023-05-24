using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace BookShop.BL.Controller
{
    public class UserController
    {
        private User User { get; }
        private bool IsNewUser { get; } = false;
        private Dictionary<Book, int> Cart { get; set; } = new Dictionary<Book, int>();

        /// <summary>
        /// Добавление нового пользователя вручную
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserController(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Пользователь не может быть null.");
            }

            User = user;
            using (BookShopDBContext db = new BookShopDBContext())
            {
                db.Users.Add(User);
                db.SaveChanges();
            }
        }


        /// <summary>
        /// Вход в аккаунт существующего пользователя
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public UserController(string email)
        {
            IsEmailValid(email);

            using (BookShopDBContext db = new BookShopDBContext())
            {
                User = db.Users
                    .Include(r => r.Role)
                    .Include(r => r.Residence)
                    .Include(s => s.SalesHistories)
                    .SingleOrDefault(u => u.Email == email)
                    ?? throw new InvalidOperationException($"Пользователя с почтой {email} не найдено.");
            }
        }


        /// <summary>
        /// Создание нового аккаунта
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="roleId"></param>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <param name="residenceId"></param>
        public UserController(string firstName, string lastName, int roleId, string email, string? phone = null, int? residenceId = null)
        {
            IsNewUser = true;

            IsFirstNameValid(firstName);
            IsLastNameValid(lastName);
            IsRoleValid(roleId);
            IsEmailValid(email);

            User = new User(firstName, lastName, roleId, email, phone, residenceId);

            using (BookShopDBContext db = new BookShopDBContext())
            {
                db.Users.Add(User);
                db.SaveChanges();
            }
        }


        public bool ChangeFirstName(string firstName)
        {
            IsFirstNameValid(firstName);

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var user = db.Users.SingleOrDefault(u => u.Id == User.Id);
                if (user != null)
                {
                    User.FirstName = firstName;
                    user.FirstName = firstName;
                    db.SaveChanges();

                    return true;
                }
                return false;
            }
        }
        public bool ChangeLastName(string lastName)
        {
            IsLastNameValid(lastName);

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var user = db.Users.SingleOrDefault(u => u.Id == User.Id);
                if (user != null)
                {
                    User.LastName = lastName;
                    user.LastName = lastName;
                    db.SaveChanges();

                    return true;
                }
                return false;
            }
        }
        public bool ChangePhone(string phone)
        {
            IsPhoneValid(phone);

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var user = db.Users.SingleOrDefault(u => u.Id == User.Id);
                if (user != null)
                {
                    User.Phone = phone;
                    user.Phone = phone;
                    db.SaveChanges();

                    return true;
                }
                return false;
            }
        }
        public bool ChangeResidence(Residence residence)
        {
            if (residence == null)
            {
                throw new ArgumentNullException("Место жительства не может быть null");
            }

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var user = db.Users.SingleOrDefault(u => u.Id == User.Id);
                if (user != null)
                {
                    User.Residence = residence;
                    user.Residence = residence;
                    db.SaveChanges();

                    return true;
                }
            }
            return false;
        }

        private bool IsFirstNameValid(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException("Имя пользователя не может быть null");
            }
            if (firstName.Length < 3)
            {
                throw new InvalidOperationException("Имя пользователя не может быть меньше трёх символов");
            }

            return true;
        }
        private bool IsLastNameValid(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException("Фамилия пользователя не может быть null");
            }
            if (lastName.Length < 3)
            {
                throw new InvalidOperationException("Фамилия пользователя не может быть меньше трёх символов");
            }

            return true;
        }
        private bool IsRoleValid(int role)
        {
            if (role == 0)
            {
                throw new InvalidOperationException("Роль пользователя не может быть равна 0");
            }

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var existingRole = db.Roles.SingleOrDefault(r => r.Id == role);
                if (existingRole == null)
                {
                    throw new InvalidOperationException("Роль не существует");
                }
            }
            return true;
        }
        private bool IsEmailValid(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("Почта пользователя не может быть null.");
            }
            if (!email.Contains('@'))
            {
                throw new InvalidOperationException("Почта пользователя обязана содержать символ '@'.");
            }
            if (!email.SkipWhile(x => x != '@').Contains('.'))
            {
                throw new InvalidOperationException("Почта пользователя обязана содержать символ '.' после символа '@'.");
            }
            if (IsNewUser)
            {
                using (BookShopDBContext db = new BookShopDBContext())
                {
                    var existingUser = db.Users.AsNoTracking().FirstOrDefault(u => u.Email == email);
                    if (existingUser != null)
                    {
                        throw new InvalidOperationException("Данная почта уже используется");
                    }
                }
            }
            return true;
        }
        private bool IsPhoneValid(string phone)
        {
            if (phone == null)
            {
                throw new ArgumentNullException("Номер телефона не может быть null");
            }

            Regex shortPattern = new Regex("[0-9]{10}");
            Regex longPattern = new Regex("[38]{2}[0-9]{10}");

            if (!(shortPattern.IsMatch(phone)) || !(longPattern.IsMatch(phone)))
            {
                throw new InvalidOperationException("Номер телефона введён некорректно.");
            }

            return true;
        }

        public ICollection<SalesHistory> GetSalesHistories()
        {
            return User.SalesHistories;
        }
        public string GetRole()
        {
            return User.Role.Name;
        }
        public string GetEmail()
        {
            return User.Email;
        }
        public string? GetPhone()
        {
            return User.Phone;
        }
        public Residence? GetResidence()
        {
            return User.Residence;
        }
        public override string ToString()
        {
            return User.ToString();
        }

        public string GetAllUser()
        {
            if (User.Role.Name == "Admin")
            {
                string usersData = "";
                using (BookShopDBContext sb = new BookShopDBContext())
                {
                    var users = sb.Users.ToList();
                    foreach (var user in users)
                    {
                        usersData += user.ToString() + '\n';
                    }
                }
                return usersData;
            }
            else 
            {
                throw new InvalidOperationException("Чтобы получить список пользователей нужно обладать правами админа");
            }
        }

        public void AddToCart(Book book, int count)
        {
            if (book == null)
            {
                throw new ArgumentNullException("Книга не может быть null");
            }
            if (count == 0)
            {
                throw new InvalidOperationException("Количество товара для покупки не может быть 0");
            }
            if (book.Count < count) 
            {
                throw new InvalidOperationException("На складе недостаточное количество товара для совершение сделки");
            }

            Cart.Add(book, count);
        }
        public void Payment()
        {
            using(BookShopDBContext db = new BookShopDBContext()) 
            {
                foreach (var books in Cart)
                {
                    var book = db.Books.SingleOrDefault(b => b.Id == books.Key.Id);
                    if (book != null)
                    {
                        book.Count -= books.Value;
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
