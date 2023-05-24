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

            UserController userController = null;

            #region Вход в существующий аккаунт или создание нового
            do
            {
                Console.WriteLine("Введите Email:");

                var email = Console.ReadLine();
                try
                {
                    userController = new UserController(email);
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
                        userController = new UserController(firstName: data.Item1,
                                                            lastName: data.Item2,
                                                            roleId: 1,              // обычный юзер
                                                            email: email,
                                                            phone: data.Item3,
                                                            residenceId: null);
                    }
                }

            } while (userController == null);
            #endregion

            Console.WriteLine($"Добро пожаловать {userController.ToString()}.");
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
            Console.WriteLine();
        }
    }
}