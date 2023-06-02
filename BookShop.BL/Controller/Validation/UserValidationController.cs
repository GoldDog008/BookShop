using BookShop.BL.Controller.IValidationData;
using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;


namespace BookShop.BL.Controller
{
    public class UserValidationController : IUserValidationData
    {
        public bool IsEmailValid(string email, bool isNewUser)
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
            if (isNewUser)
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
        public bool IsFirstNameValid(string firstName)
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
        public bool IsLastNameValid(string lastName)
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
        public bool IsPhoneValid(string phone)
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
        public bool IsRoleValid(int role)
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
        public bool IsUserIdValid(int userId)
        {
            if (userId < 1)
            {
                throw new InvalidOperationException("Id пользователя не может меньше 1");
            }

            using (BookShopDBContext db = new BookShopDBContext())
            {
                var existingRole = db.Users.SingleOrDefault(u => u.Id == userId);
                if (existingRole == null)
                {
                    throw new InvalidOperationException("Пользователь не существует");
                }
            }
            return true;
        }
    }
}
