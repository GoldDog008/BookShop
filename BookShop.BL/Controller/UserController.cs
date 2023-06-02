using BookShop.BL.Controller.IValidationData;
using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BookShop.BL.Controller
{
    public class UserController
    {
        protected IUserValidationData _validationData = new UserValidationController();

        /// <summary>
        /// Пользователь
        /// </summary>
        protected User User { get; }

        /// <summary>
        /// Место жительства пользователя
        /// </summary>
        protected ResidenceController Residence { get; set; }

        /// <summary>
        /// История продаж пользователя
        /// </summary>
        protected SalesHistoryController Sale { get; set; }

        /// <summary>
        /// Корзина пользователя
        /// </summary>
        protected Dictionary<int, int> Cart { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// Является ли пользователь новым
        /// </summary>
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
            IsEmailValid(email, IsNewUser);

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
            IsEmailValid(email, IsNewUser);

            User = new User(firstName, lastName, roleId, email, phone);
            Residence = new ResidenceController(User, null);

            using (BookShopDBContext db = new BookShopDBContext())
            {
                db.Users.Add(User);
                db.SaveChanges();
            }
        }



        /// <summary>
        /// Изменить данные о пользователе
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="phone"></param>
        /// <param name="region"></param>
        /// <param name="city"></param>
        /// <param name="street"></param>
        /// <param name="houseNumber"></param>
        /// <param name="apartmentNumber"></param>
        /// <returns></returns>
        public async Task<bool> ChangeUserDataAsync(string? firstName = null,
                                   string? lastName = null,
                                   string? phone = null,
                                   string? region = null,
                                   string? city = null,
                                   string? street = null,
                                   int? houseNumber = null,
                                   int? apartmentNumber = null)
        {
            bool isDataChanged = false;

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var user = await db.Users.SingleOrDefaultAsync(u => u.Id == User.Id);
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
                        await Residence.ChangeResidenceDataAsync(region: region);
                        isDataChanged = true;
                    }
                    if (city != null)
                    {
                        await Residence.ChangeResidenceDataAsync(city: city);
                        isDataChanged = true;
                    }
                    if (street != null)
                    {
                        await Residence.ChangeResidenceDataAsync(street: street);
                        isDataChanged = true;
                    }
                    if (houseNumber != null)
                    {
                        await Residence.ChangeResidenceDataAsync(houseNumber: houseNumber);
                        isDataChanged = true;
                    }
                    if (apartmentNumber != null)
                    {
                        await Residence.ChangeResidenceDataAsync(apartmentNumber: apartmentNumber);
                        isDataChanged = true;
                    }

                    if (isDataChanged)
                    {
                        db.SaveChanges();
                    }
                }
                return isDataChanged;
            }
        }


        /// <summary>
        /// Проверка на корректность имени
        /// </summary>
        /// <param name="firstName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        protected bool IsFirstNameValid(string firstName)
        {
            return _validationData.IsFirstNameValid(firstName);
        }

        /// <summary>
        /// Проверка на корректность фамилии
        /// </summary>
        /// <param name="lastName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        protected bool IsLastNameValid(string lastName)
        {
            return _validationData.IsLastNameValid(lastName);
        }

        /// <summary>
        /// Проверка на корректность роли
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected bool IsRoleValid(int role)
        {
            return _validationData.IsRoleValid(role);
        }

        /// <summary>
        /// Проверка на корректность почты
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        protected bool IsEmailValid(string email, bool isNewUser)
        {
            return _validationData.IsEmailValid(email, isNewUser);
        }

        /// <summary>
        /// Проверка на корректность номера телефона
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        protected bool IsPhoneValid(string phone)
        {
            return _validationData.IsPhoneValid(phone);
        }
        protected bool IsUserIdValid(int userId) 
        {
            return _validationData.IsUserIdValid(userId);
        }


        /// <summary>
        /// Получить историю продаж пользователя
        /// </summary>
        /// <returns></returns>
        public ICollection<SalesHistory> GetSalesHistories()
        {
            return User.SalesHistories;
        }

        /// <summary>
        /// Получить место жительства пользователя
        /// </summary>
        /// <returns></returns>
        public Residence? GetResidence()
        {
            return User.Residence;
        }

        public override string ToString()
        {
            return User.ToString();
        }

        /// <summary>
        /// Получить всю информацию о пользователе
        /// </summary>
        /// <returns></returns>
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



        /// <summary>
        /// Добавить товар в корзину
        /// </summary>
        /// <param name="book"></param>
        /// <param name="count"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddToCart(int bookId, int count)
        {
            if (count <= 0)
            {
                throw new InvalidOperationException("Количество товара для покупки не может быть меньше 1");
            }

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var existingBook = db.Books.AsNoTracking().SingleOrDefault(b => b.Id == bookId);

                if (existingBook == null)
                {
                    throw new InvalidOperationException($"Книга с индексом {bookId} не найдена");
                }

                if (existingBook.Count < count)
                {
                    throw new InvalidOperationException("На складе недостаточное количество товара для совершения сделки");
                }
            }

            Cart.Add(bookId, count);
        }

        /// <summary>
        /// Удалить товар из корзины
        /// </summary>
        /// <param name="bookId"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void DeleteFromCart(int bookId)
        {
            if (Cart.ContainsKey(bookId))
            {
                Cart.Remove(bookId);
            }
            else
            {
                throw new InvalidOperationException("Данной книги нет в корзине");
            }
        }

        /// <summary>
        /// Совершить покупку
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Payment()
        {
            if (!Residence.IsAllDataAreFilled())
            {
                throw new InvalidOperationException("Невозможно завершить заказ, потому что не все данные о месте жительства заполнены");
            }
            if (Cart.Count == 0)
            {
                throw new InvalidOperationException("В корзине нет товаров");
            }

            using (BookShopDBContext db = new BookShopDBContext())
            {
                foreach (var books in Cart)
                {
                    var book = db.Books.SingleOrDefault(b => b.Id == books.Key);
                    if (book != null)
                    {
                        Sale = new SalesHistoryController(book.Id, User.Id, books.Value, book.Price * books.Value, DateTime.Now);
                        book.Count -= books.Value;

                        db.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Получить товары из корзины
        /// </summary>
        /// <returns></returns>
        public string GetItemsFromCart()
        {
            StringBuilder books = new StringBuilder();

            using (BookShopDBContext db = new BookShopDBContext())
            {
                foreach (var book in Cart)
                {
                    books.Append($"Книга: {db.Books.SingleOrDefault(b => b.Id == book.Key)?.Name} Количество: {book.Value}\n");
                }
            }

            return books.ToString();
        }

        /// <summary>
        /// Является ли пользователь админом
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin()
        {
            return User.RoleId == 2;
        }
    }
}
