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
            //canek.kryt.12@gmail.com
            //asdqwert@mail.ru                                  
            UserController controller = null;
            BookController bookController = null;

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

            //Residence residence = new Residence("Харьковская область", "Донец", "Спортивная", 32, 8);
            await controller.ChangeUserDataAsync(region: "Харьковская область", 
                                                city: "Донец", 
                                                street: "Спортивная", 
                                                houseNumber: 32, 
                                                apartmentNumber: 8);

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

                    case "q":
                        var allBooks = BookController.GetAllBook();
                        Console.WriteLine(allBooks);
                        break;

                    case "w":
                        Console.WriteLine(controller.GetAllInformationAboutUser());
                        break;

                    case "e":
                        PrintChangeUserDataMenu();
                        string changeChoice = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");


                        break;
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
            Console.WriteLine("C - Редактировать существующую книгу");
            Console.WriteLine("V - Удалить пользователя");
            Console.WriteLine("B - Посмотреть весь список пользователей");
            Console.WriteLine("N - Получить историю продаж");

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
            Console.WriteLine("W - Посмотреть всю информацию о себе");
            Console.WriteLine("E - Изменить информацию о себе");
            Console.WriteLine("R - Добавить книгу в корзину");
            Console.WriteLine("T - Посмотреть корзину");
            Console.WriteLine("Y - Совершить покупку");

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
            Console.WriteLine("R - Место жительства");
            Console.WriteLine("T - Город жительства");
            Console.WriteLine("Y - Номер дома");
            Console.WriteLine("U - Номер квартиры");
        }
        static async void ChangeUserData(string changeChoice, UserController user)
        {
            switch (changeChoice.ToLower()) 
            {
                case "q":
                    Console.WriteLine("Введите новое имя:");
                    string firstName = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                    await user.ChangeUserDataAsync(firstName: firstName);
                    break;

                case "w":
                    Console.WriteLine("Введите новою фамилию:");
                    string lastName = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                    await user.ChangeUserDataAsync(lastName: lastName);
                    break;

                case "e":
                    Console.WriteLine("Введите новый номер телефона:");
                    string phone = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                    user.ChangeUserDataAsync(phone: phone);
                    break;

                case "r":
                    string houseNumber, apartmentNumber;
                    int houseNum, apartNum;

                    Console.WriteLine("Введите область:");
                    string region = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                    Console.WriteLine("Введите город:");
                    string city = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                    Console.WriteLine("Введите улицу:");
                    string street = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                    
                    do
                    {
                        Console.WriteLine("Введите номер дома:");
                        houseNumber = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                    } while (int.TryParse(houseNumber, out houseNum));

                    do
                    {
                        Console.WriteLine("Введите номер дома:");
                        apartmentNumber = Console.ReadLine() ?? throw new ArgumentNullException("Ввод не может быть null");

                    } while (int.TryParse(apartmentNumber, out apartNum));

                    if (user.GetResidence() == null)
                    {
                        //Residence residence = new Residence();
                    }
                    else
                    {
                       // user.ChangeUserData(phone: phone);
                    }
                    
                    break;
                default:
                    Console.WriteLine("Некорректные данные");
                    break;
            }
        }
    }
}