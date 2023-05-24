using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookShop.BL.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.BL.Model;

namespace BookShop.BL.Controller.Tests
{
    [TestClass()]
    public class UserControllerTests
    {
        [TestMethod()]
        public void ChangeFirstNameTest()
        {
            //Arrange 
            var email = "canek.kryt.12@gmail.com";


            //Act
            UserController userController = new UserController(email);
            //userController.ChangeFirstName(changedFirstName);

            //Assert
            //Assert.AreEqual(changedFirstName, userController.User.FirstName);
        }

        [TestMethod()]
        public void ChangeLastNameTest()
        {
            
            Assert.Fail();
        }

        [TestMethod()]
        public void ChangePhoneTest()
        {
            Assert.Fail();
        }
    }
}