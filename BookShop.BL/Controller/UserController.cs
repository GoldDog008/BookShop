using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace BookShop.BL.Controller
{
    public class UserController
    {
        protected User User { get; }
        protected ResidenceController Residence { get; set; }
        protected Dictionary<Book, int> Cart { get; set; } = new Dictionary<Book, int>();
        private bool IsNewUser { get; } = false;

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
            Residence = new ResidenceController(user);

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

                if (User.Residence != null)
                {
                    Residence = new ResidenceController(User);
                }
                else 
                {
                    Residence = new ResidenceController(User, null);
                }
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
        public UserController(string firstName, string lastName, int roleId, string email, string? phone = null)
        {
            IsNewUser = true;

            IsFirstNameValid(firstName);
            IsLastNameValid(lastName);
            IsRoleValid(roleId);
            IsEmailValid(email);

            User = new User(firstName, lastName, roleId, email, phone);

            using (BookShopDBContext db = new BookShopDBContext())
            {
                db.Users.Add(User);
                db.SaveChanges();
            }
        }

        public bool ChangeUserData(string firstName = null, 
                                   string lastName = null, 
                                   string phone = null, 
                                   string? region = null, 
                                   string? city = null, 
                                   string? street = null, 
                                   int? houseNumber = null, 
                                   int? apartmentNumber = null)
        {
            bool isDataChanged = false;

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var user = db.Users.SingleOrDefault(u => u.Id == User.Id);
                if (user != null)
                {
                    if (firstName != null)
                    {
                        IsFirstNameValid(firstName);
                        User.FirstName = firstName;
                        user.FirstName = firstName;
                        isDataChanged = true;
                    }
                    if (lastName != null)
                    {
                        IsLastNameValid(lastName);
                        User.LastName = lastName;
                        user.LastName = lastName;
                        isDataChanged = true;
                    }
                    if (phone != null)
                    {
                        IsPhoneValid(phone);
                        User.Phone = phone;
                        user.Phone = phone;
                        isDataChanged = true;
                    }
                    if (region != null)
                    {
                        Residence.ChangeResidenceData(region: region);
                        isDataChanged = true;
                    }
                    if (city != null)
                    {
                        Residence.ChangeResidenceData(city: city);
                        isDataChanged = true;
                    }
                    if (street != null)
                    {
                        Residence.ChangeResidenceData(street: street);
                        isDataChanged = true;
                    }
                    if (houseNumber != null)
                    {
                        Residence.ChangeResidenceData(houseNumber: houseNumber);
                        isDataChanged = true;
                    }
                    if (apartmentNumber != null)
                    {
                        Residence.ChangeResidenceData(apartmentNumber: apartmentNumber);
                        isDataChanged = true;
                    }

                    if (isDataChanged)
                    {
                        db.SaveChanges();
                    }

                    return isDataChanged;
                }
                return false;
            } 
        }



        protected bool IsFirstNameValid(string firstName)
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
        protected bool IsLastNameValid(string lastName)
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
        protected bool IsRoleValid(int role)
        {
            if (role < 1)
            {
                throw new InvalidOperationException("Роль пользователя не может меньше 1");
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
        protected bool IsEmailValid(string email)
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
        protected bool IsPhoneValid(string phone)
        {
            if (phone == null)
            {
                throw new ArgumentNullException("Номер телефона не может быть null");
            }

            Regex shortPattern = new Regex("[0-9]{10}");        //095 123 45 67
            Regex longPattern = new Regex("[38]{2}[0-9]{10}");  //38 095 123 45 67

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
        public Residence? GetResidence()
        {
            return User.Residence;
        }
        public override string ToString()
        {
            return User.ToString();
        }
        public string GetAllInformationAboutUser()
        {
            return $"Id: {User.Id}, Имя: {User.ToString()}\n" +
                $"Телефон: {User.Phone ?? "Нет данных"}\n" +
                $"Почта: {User.Email}\n" +
                $"Роль: {User.Role.Name}\n" +
                $"Область: {User?.Residence?.Region ?? "Нет данных"}\n" +
                $"Город: {User?.Residence?.City ?? "Нет данных"}\n" +
                $"Номер дома: {User?.Residence?.HouseNumber.ToString() ?? "Нет данных"}\n" +
                $"Номер квартиры: {User?.Residence?.ApartmentNumber.ToString() ?? "Нет данных"}\n";
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
        public bool IsAdmin()
        {
            return User.RoleId == 2;
        }
    }
}
