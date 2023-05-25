using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookShop.BL.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.BL.Controller.Tests
{
    [TestClass()]
    public class UserControllerTests
    {
        [TestMethod()]
        public void UserControllerTest()
        {
            //Arrange 
            string firstName = Guid.NewGuid().ToString();
            string lastName = Guid.NewGuid().ToString();
            int roleId = 1;
            string email = "test@gmail.com";
            string? phone = null;
            int? residenceId = null;


            //Act 
            //UserController user;
        }
    }
}
