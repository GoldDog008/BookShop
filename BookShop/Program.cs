using BookShop.BL.Controller;
using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Linq;
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
                var email = Console.ReadLine();

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
                    //case "b" when controller is AdminController:
                    //    //Console.WriteLine(controller.GetA);
                    //    break;


                    case "q": // Посмотреть список всех книг
                        {
                            var allBooks = BookController.GetAllBook();
                            Console.WriteLine(allBooks);
                            break;
                        }
                    case "w": // Найти опреденную книгу
                        {
                            Console.WriteLine("Введите книгу, которую нужно найти");
                            string bookName = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                            BookController book = new BookController(bookName);
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
                                authorBooks = AuthorController.GetAllBooksByAuthorAsync(name); 
                                Console.WriteLine(authorBooks);
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
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
                            string changeChoice = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");
                            ChangeUserData(changeChoice, controller);

                            Console.WriteLine("Данные были успешно изменены");
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
                                book = new BookController(bookName);
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            controller.AddToCart(book, bookCount);
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
                    default:
                        Console.WriteLine("Некорректные данные");
                        continue;
                }
            }
        }

        /// <summary>
        /// Ввод данных для нового пользователя
        /// </summary>
        /// <returns></returns>
        static (string, string, string?) InputDataForNewUser()
        {
            Console.Write("Введите имя пользователя: ");
            var FirstName = Console.ReadLine();

            Console.Write("Введите фамилию пользователя: ");
            var LastName = Console.ReadLine();

            string? Phone = null;
            do
            {
                Console.Write("Введите телефон пользователя: ");
                Phone = Console.ReadLine();

                Regex shortPattern = new Regex("[0-9]{10}");
                Regex longPattern = new Regex("[38]{2}[0-9]{10}");

                if (Phone == "" || Phone == " ")
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
        static async void ChangeUserData(string changeChoice, UserController user)
        {
            string firstName, lastName, phone, region, city, street;
            string inputNumber;
            int houseNumber, apartmentNumber;

            switch (changeChoice.ToLower())
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
                    break;
            }
        }
    }
}