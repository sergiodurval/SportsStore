using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SportsStore.Domain.Entities;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //arrange - criando produtos para teste
            Mock<IProductsRepository> Mock = new Mock<IProductsRepository>();
            Mock.Setup(m => m.Products).Returns(new Product[] 
            {
                new Product{ProductID = 1 , Name = "P1", Category = "Apples" }
            }.AsQueryable());

            //arrange - criando um carrinho
            Cart cart = new Cart();

            //arrange
            CartController target = new CartController(Mock.Object,null); 

            //act
            target.AddToCart(cart, 1, null);

            //assert
            Assert.AreEqual(cart.Lines.Count(), 1);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
           //arrange -criando produtos para teste
           Product p1 = new Product { ProductID = 1, Name = "P1" };
           Product p2 = new Product { ProductID = 2, Name = "P2" };

           //arrange
           Cart target = new Cart();

           //act
           target.AddItem(p1, 1);
           target.AddItem(p2, 1);
           target.AddItem(p1, 10);
           CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

          //assert
           Assert.AreEqual(results.Length, 2);
           Assert.AreEqual(results[0].Quantity, 11);
           Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            //arrange - criando produtos para teste
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            //arrange
            Cart target = new Cart();

            //arrange - adicionando produtos ao carrinho
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            //act
            target.RemoveLine(p2);

            //assert
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //arrange
            Product p1 = new Product {ProductID = 1 ,Name = "P1",Price = 100m };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50m };

            //arrange
            Cart target = new Cart();

            //act -adiciona os produtos
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            //assert
            Assert.AreEqual(450m, result);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
           //arrange
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100m };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50m };

            //arrange
            Cart target = new Cart();

            //arrange - adicionando os produtos
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            //act - resetando o carrinho
            target.Clear();

            //assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                 new Product {ProductID = 1 , Name = "P1" , Category = "Apples"}
            }.AsQueryable());

            //arrange - criando carrinho
            Cart cart = new Cart();

            //arrange
            CartController target = new CartController(mock.Object,null);

            //act
            RedirectToRouteResult result = target.AddToCart(cart,2, "myUrl");

            //assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
            
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            //arrange
            Cart cart = new Cart();

            //arrange
            CartController target = new CartController(null,null);

            //act
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            //assert
            Assert.AreSame(result.Cart,cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            //arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            //arrange - criando o carrinho vazio
            Cart cart = new Cart();

            //arrange
            ShippingDetails shippingDetails = new ShippingDetails();
            
            //arrange
            CartController target = new CartController(null, mock.Object);

            //act
            ViewResult result = target.Checkout(cart, shippingDetails);

            //assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }
    }
}
