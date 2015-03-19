using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;
namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminSecurityTests
    {
        [TestMethod]
        public void Can_Login_With_Valid_Credentials()
        {
            //arrange -criando os mock objects de autenticação
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "secret")).Returns(true);

            //arrange - criando a viewmodel
            LoginViewModel model = new LoginViewModel {
                UserName = "admin",
                Password = "secret"
            };

           //arrange - criando a controller
            AccountController target = new AccountController(mock.Object);

            //act -  autenticando usando credenciais válidas.
            ActionResult result = target.Login(model, "/MyUrl");

            //asssert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyUrl",((RedirectResult)result).Url);
        }

        [TestMethod]
        public void Cannot_Login_With_Invalid_Credentials()
        {
            //arrange - mock objects de autenticação
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("badUser", "badPass")).Returns(false);

            //arrange - criando a viewmodel
            LoginViewModel model = new LoginViewModel {
                UserName = "badUser",
                Password = "badPass"
            };

            //arrange - criando a controller
            AccountController target = new AccountController(mock.Object);

            //act - autenticando o usuário
            ActionResult result = target.Login(model, "MyUrl");

            //assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}
