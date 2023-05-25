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
        static void Main(string[] args)
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
                                                            phone: data.Item3,
                                                            residenceId: null);
                    }
                }

            } while (controller == null);
            #endregion

            Console.WriteLine($"Добро пожаловать {controller.ToString()}.");
            while (true)
            {
                if (controller is AdminController admin) 
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
                    case "x" when controller is AdminController:
                        Console.WriteLine("Добавить новую книгу");
                        break;

                    case "q":
                        var allBooks = BookController.GetAllBook();
                        Console.WriteLine(allBooks);
                        break;

                    default:
                        Console.WriteLine("Некорректные данные");
                        continue;
                }
            }
        }

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
        static void PrintAdminMenu()
        {
            PrintUserMenu();

            Console.WriteLine();
            Console.WriteLine("Z - Изменить пользователю права");
            Console.WriteLine("X - Добавить новую книгу");
            Console.WriteLine("C - Редактировать существующую книгу");
            Console.WriteLine("V - Удалить пользователя");
            Console.WriteLine("B - Посмотреть весь список пользователей");
            Console.WriteLine("N - Получить историю продаж");
        }
        static void PrintUserMenu()
        {
            Console.WriteLine("Что вы хотите сделать?");
            Console.WriteLine("Q - Посмотреть весь список книг");
            Console.WriteLine("W - Посмотреть всю информацию о себе");
            Console.WriteLine("E - Изменить информацию о себе");
            Console.WriteLine("R - Добавить книгу в корзину");
            Console.WriteLine("T - Посмотреть корзину");
            Console.WriteLine("Y - Совершить покупку");
        }
    }
}