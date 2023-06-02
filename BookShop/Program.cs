using BookShop.BL.Controller;
using System.Text.RegularExpressions;

namespace BookShop
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в МОЙ книжный магазин у которого ещё нет названия)");

            //canek.kryt.12@gmail.com WarAndPeace  Данте Алигьери                                     
            //asdqwert@mail.ru

            UserController controller = null;

            #region Вход в существующий аккаунт или создание нового
            do
            {
                Console.WriteLine("Введите Email:");
                var email = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                try
                {
                    controller = new UserController(email);
                    if (controller.IsAdmin())
                    {
                        controller = new AdminController(email);
                    }
                }

                #region Ловля основных ошибок ввода пользователя
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion

                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);

                    Console.WriteLine($"Хотите ли вы создать новый аккаунт с почтой {email} (Y/N)?");

                    if (Console.ReadLine()?.ToLower() == "y")
                    {
                        var data = InputDataForNewUser();
                        controller = new UserController(firstName: data.Item1,
                                                            lastName: data.Item2,
                                                            roleId: 1,              // обычный юзер
                                                            email: email,
                                                            phone: data.Item3);
                    }
                }

            } while (controller == null);
            #endregion

            Console.WriteLine($"Добро пожаловать {controller.ToString()}.");

            while (true)
            {
                if (controller is AdminController)
                {
                    PrintAdminMenu();
                }
                else
                {
                    PrintUserMenu();
                }

                var choice = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                switch (choice.ToLower())
                {
                    #region Выбор для обычного пользователя
                    case "q": // Посмотреть список всех книг
                        {
                            var allBooks = BookController.GetAllBookInformation();
                            Console.WriteLine(allBooks);
                            break;
                        }
                    case "w": // Найти опреденную книгу
                        {
                            Console.WriteLine("Введите книгу, которую нужно найти");
                            string bookName = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");
                            BookController book = null;

                            try
                            {
                                book = new BookController(bookName);
                            }
                            catch (ArgumentNullException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }

                            Console.WriteLine($"Цена: {book.GetPrice()}\n" +
                                              $"Количество на складе: {book.GetCount()}\n" +
                                              $"Автор: {book.GetAuthor()}\n" +
                                              $"Описание: {book.GetDescription()}");
                            break;
                        }
                    case "e": // Посмотреть все книги от определенного автора
                        {
                            Console.WriteLine("Введите автора:");
                            string name = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");
                            string authorBooks = "";

                            try
                            {
                                authorBooks = AuthorController.GetAllBooksByAuthor(name);
                                Console.WriteLine(authorBooks);
                            }
                            catch (ArgumentNullException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }

                            break;
                        }
                    case "r": // Посмотреть всю информацию о себе
                        {
                            Console.WriteLine(controller.GetAllInformationAboutUser());
                            break;
                        }
                    case "t": // Изменить информацию о себе
                        {
                            PrintChangeUserDataMenu();
                            string userInput = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");
                            bool IsChanged = false;

                            try
                            {
                                IsChanged = await ChangeUserData(userInput, controller);
                            }
                            catch (ArgumentNullException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }

                            if (IsChanged)
                            {
                                Console.WriteLine("Данные были успешно изменены");
                            }
                            else
                            {
                                Console.WriteLine("Данные изменены не были");
                            }
                            break;
                        }
                    case "y": // Добавить книгу в корзину
                        {
                            BookController book = null;
                            int bookCount;
                            string inputCount;

                            Console.WriteLine("Введите название книги:");
                            string bookName = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                            do
                            {
                                Console.WriteLine("Введите количество книг которые вы хотите купить:");
                                inputCount = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                            } while (!int.TryParse(inputCount, out bookCount));

                            try
                            {
                                book = GetBook(bookName);
                                controller.AddToCart(book.GetId(), bookCount);
                            }
                            catch (ArgumentNullException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }

                            Console.WriteLine($"Книга с названием {book.ToString()} была добавлена в корзину");

                            break;
                        }
                    case "u": // Посмотреть корзину
                        {
                            var cart = controller.GetItemsFromCart();
                            Console.WriteLine(cart);
                            break;
                        }
                    case "i": // Удалить товар из корзины
                        {
                            Console.WriteLine("Введите название книги:");

                            string bookName = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                            try
                            {
                                BookController book = GetBook(bookName);
                                controller.DeleteFromCart(book.GetId());
                            }
                            catch (ArgumentNullException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }

                            Console.WriteLine("Книга была успешна удалена из корззины");
                            break;
                        }
                    case "o": // Совершить покупку
                        {
                            try
                            {
                                controller.Payment();
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }

                            Console.WriteLine("Покупка успешна");
                            break;
                        }
                    #endregion

                    #region Выбор для админа
                    case "z" when controller is AdminController admin: // Изменить пользователю права
                        {
                            int userId, inputId;
                            string inputData;

                            do
                            {
                                Console.WriteLine("Введите id пользователя, которому нужно поменять роль");
                                inputData = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                            } while (!int.TryParse(inputData, out userId));

                            do
                            {
                                Console.WriteLine("Введите новый id роли для пользователя");
                                inputData = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                            } while (!int.TryParse(inputData, out inputId));

                            try
                            {
                                admin.ChangeUserRole(admin.GetUser(userId), inputId);
                            }
                            catch (ArgumentNullException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }

                            Console.WriteLine("Роль успешно изменена");
                            break;
                        }
                    case "x" when controller is AdminController admin: // Добавить новую книгу
                        {
                            var bookData = InputDataForNewBook();

                            try
                            {
                                admin.AddNewBook(bookData.Item1, // Название книги
                                             bookData.Item2, // Цена
                                             bookData.Item3, // Количество
                                             bookData.Item4); // Описание
                            }
                            catch (ArgumentNullException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }

                            break;
                        }
                    case "c" when controller is AdminController admin: //Редактировать существующую книгу
                        {
                            Console.WriteLine("Эта возможность разрабатывается(нет)");
                            break;
                        }
                    case "v" when controller is AdminController admin: // Удалить пользователя
                        {
                            int userId;
                            string inputData;

                            do
                            {
                                Console.WriteLine("Введите Id пользователя, которого нужно удалить");
                                inputData = Console.ReadLine();

                            } while (!int.TryParse(inputData, out userId));

                            try
                            {
                                await admin.DeleteUserAsync(userId);
                            }
                            catch (ArgumentNullException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }

                            Console.WriteLine("Удаление пользователя успешно");
                            break;
                        }
                    case "b" when controller is AdminController admin: // Посмотреть весь список пользователей
                        {
                            Console.WriteLine(admin.GetAllUser());
                            break;
                        }
                    case "n" when controller is AdminController admin: // Получить историю продаж пользователя
                        {
                            int userId;
                            string inputData;

                            do
                            {
                                Console.WriteLine("Введите Id пользователя");
                                inputData = Console.ReadLine();

                            } while (!int.TryParse(inputData, out userId));

                            try
                            {
                                Console.WriteLine(await admin.GetUserSalesHistoryAsync(userId));
                            }
                            catch (ArgumentNullException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                                break;
                            }

                            break;
                        }
                    #endregion

                    default:
                        Console.WriteLine("Некорректные данные");
                        continue;
                }
            }
        }

        /// <summary>
        /// Ввод данных для нового пользователя
        /// </summary>
        /// <returns>Имя, фамилия, телефон</returns>
        static (string, string, string?) InputDataForNewUser()
        {
            Console.Write("Введите имя пользователя: ");
            string FirstName = Console.ReadLine();

            Console.Write("Введите фамилию пользователя: ");
            string LastName = Console.ReadLine();

            string? Phone = null;
            do
            {
                Console.Write("Введите телефон пользователя: ");
                Phone = Console.ReadLine();

                Regex shortPattern = new Regex("[0-9]{10}");
                Regex longPattern = new Regex("[38]{2}[0-9]{10}");

                if (Phone == "")
                {
                    Phone = null;
                    break;
                }
                else if (!(shortPattern.IsMatch(Phone)) || !(longPattern.IsMatch(Phone)))
                {
                    Console.WriteLine("Номер телефона введён некорректно.");
                }
                else
                {
                    break;
                }

            } while (true);

            return (FirstName, LastName, Phone);
        }

        /// <summary>
        /// Ввод данных для новой книги
        /// </summary>
        /// <returns>Название книги, цена, количество, описание</returns>
        static (string, int, int, string?) InputDataForNewBook()
        {
            int price, count, authorId;
            string inputData;

            Console.Write("Введите название книги: ");
            string name = Console.ReadLine();

            do
            {
                Console.Write("Введите цену книги: ");
                inputData = Console.ReadLine();

            } while (!int.TryParse(inputData, out price));

            do
            {
                Console.Write("Введите количество книг: ");
                inputData = Console.ReadLine();

            } while (!int.TryParse(inputData, out count));

            Console.Write("Введите описание книги: ");
            string? description = Console.ReadLine();

            if (description == "")
            {
                description = null;
            }
            return (name, price, count, description);
        }

        /// <summary>
        /// Отобразить меню для админа
        /// </summary>
        static void PrintAdminMenu()
        {
            PrintUserMenu();

            Console.ForegroundColor = ConsoleColor.DarkRed;

            Console.WriteLine();
            Console.WriteLine("Z - Изменить пользователю права");
            Console.WriteLine("X - Добавить новую книгу");
            Console.WriteLine("C - Редактировать существующую книгу (не реализовано)");
            Console.WriteLine("V - Удалить пользователя");
            Console.WriteLine("B - Посмотреть весь список пользователей");
            Console.WriteLine("N - Получить историю продаж пользователя");

            Console.ResetColor();
        }

        /// <summary>
        /// Отобразить меню для пользователя
        /// </summary>
        static void PrintUserMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Console.WriteLine("Что вы хотите сделать?");
            Console.WriteLine("Q - Посмотреть весь список книг");
            Console.WriteLine("W - Найти опреденную книгу");
            Console.WriteLine("E - Посмотреть все книги от определенного автора");
            Console.WriteLine("R - Посмотреть всю информацию о себе");
            Console.WriteLine("T - Изменить информацию о себе");
            Console.WriteLine("Y - Добавить книгу в корзину");
            Console.WriteLine("U - Посмотреть корзину");
            Console.WriteLine("I - Удалить товар из корзины");
            Console.WriteLine("O - Совершить покупку");

            Console.ResetColor();
        }

        /// <summary>
        /// Отобразить меню для сменны данных о пользователе
        /// </summary>
        static void PrintChangeUserDataMenu()
        {
            Console.WriteLine("Что вы хотите изменить?");
            Console.WriteLine("Q - Имя");
            Console.WriteLine("W - Фамилия");
            Console.WriteLine("E - Телефон");
            Console.WriteLine("R - Область");
            Console.WriteLine("T - Город");
            Console.WriteLine("Y - Улица");
            Console.WriteLine("U - Номер дома");
            Console.WriteLine("I - Номер квартиры");
            Console.WriteLine("U - Выйти из этого меню");
        }

        /// <summary>
        /// Ввод новых данных для данных о пользователе
        /// </summary>
        /// <param name="changeChoice"></param>
        /// <param name="user"></param>
        /// <exception cref="ArgumentNullException"></exception>
        static async Task<bool> ChangeUserData(string userInput, UserController user)
        {
            string firstName, lastName, phone, region, city, street;
            string inputNumber;
            int houseNumber, apartmentNumber;

            try
            {
                switch (userInput.ToLower())
                {
                    case "q": // Смена имени
                        {
                            Console.WriteLine("Введите новое имя:");

                            firstName = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");
                            await user.ChangeUserDataAsync(firstName: firstName);
                            break;
                        }

                    case "w": // Смена фамилии
                        {
                            Console.WriteLine("Введите новою фамилию:");

                            lastName = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");
                            await user.ChangeUserDataAsync(lastName: lastName);
                            break;
                        }

                    case "e": // Смена номера телефона
                        {
                            Console.WriteLine("Введите новый номер телефона:");

                            phone = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");
                            await user.ChangeUserDataAsync(phone: phone);
                            break;
                        }

                    case "r": // Смена области
                        {
                            Console.WriteLine("Введите область:");

                            region = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");
                            await user.ChangeUserDataAsync(region: region);
                            break;
                        }

                    case "t": // Смена города
                        {
                            Console.WriteLine("Введите город:");

                            city = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");
                            await user.ChangeUserDataAsync(city: city);
                            break;
                        }

                    case "y": // Смена улицы
                        {
                            Console.WriteLine("Введите улицу:");

                            street = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");
                            await user.ChangeUserDataAsync(street: street);
                            break;
                        }

                    case "u": // Смена номера дома
                        {
                            do
                            {
                                Console.WriteLine("Введите номер дома:");
                                inputNumber = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                            } while (!int.TryParse(inputNumber, out houseNumber));

                            await user.ChangeUserDataAsync(houseNumber: houseNumber);
                            break;
                        }

                    case "i": // Смена номера квартиры
                        {
                            do
                            {
                                Console.WriteLine("номер квартиры:");
                                inputNumber = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                            } while (!int.TryParse(inputNumber, out apartmentNumber));

                            await user.ChangeUserDataAsync(apartmentNumber: apartmentNumber);
                            break;
                        }

                    default:
                        Console.WriteLine("Некорректные данные");
                        return false;
                }
                return true;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        static BookController GetBook(string bookName)
        {
            return new BookController(bookName);
        }
    }
}