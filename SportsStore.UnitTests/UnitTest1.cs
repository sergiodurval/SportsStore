using SportsStore.WebUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SportsStore.Domain.Abstract;
using System.Web.Mvc;
using Moq;
using SportsStore.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;
namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "p1"},
                new Product {ProductID = 2, Name = "p2"},
                new Product {ProductID = 3, Name = "p3"},
                new Product {ProductID = 4, Name = "p4"},
                new Product {ProductID = 5, Name = "p5"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;
            
            //act
            //IEnumerable<Product> result = (IEnumerable<Product>)controller.List(null,2);
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;
            
            //assert
            Product[] productArray = result.Products.ToArray();
            Assert.IsTrue(productArray.Length == 2);
            Assert.AreEqual(productArray[0].Name, "p4");
            Assert.AreEqual(productArray[1].Name, "p5");
        }

        [TestMethod]
        public void Can_Generate_PageLinks()
        {
           //arrange
            HtmlHelper myHelper = null;

            //arrange
            PagingInfo pagingInfo = new PagingInfo{ 
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage =10
            };

            //arrange
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                + @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "p1"},
                new Product {ProductID = 2, Name = "p2"},
                new Product {ProductID = 3, Name = "p3"},
                new Product {ProductID = 4, Name = "p4"},
                new Product {ProductID = 5, Name = "p5"}
            });

            //arrange
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //act - parei aqui -Concertar esse teste depois
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;
            PagingInfo PagingInfo = result.PagingInfo;
            
            //assert
            Assert.AreEqual(PagingInfo.CurrentPage, 2);
            Assert.AreEqual(PagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(PagingInfo.TotalItems, 5);
            Assert.AreEqual(PagingInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            //arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "p1" ,Category = "Cat1"},
                new Product {ProductID = 2, Name = "p2",Category = "Cat2"},
                new Product {ProductID = 3, Name = "p3",Category = "Cat1"},
                new Product {ProductID = 4, Name = "p4",Category = "Cat2"},
                new Product {ProductID = 5, Name = "p5",Category = "Cat3"}
            });

            //arrange
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //act
            Product[] result = ((ProductsListViewModel)controller.List("Cat2",1).Model).Products.ToArray();

            //assert
            Assert.AreEqual(result.Length,2);
            Assert.IsTrue(result[0].Name == "p2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "p4" && result[0].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] 
            {
                new Product {ProductID = 1, Name = "p1" ,Category = "Apples"},//usando distinct(0)
                new Product {ProductID = 2, Name = "p2",Category = "Apples"},
                new Product {ProductID = 3, Name = "p3",Category = "Plums"},//usando distinct(2)
                new Product {ProductID = 4, Name = "p4",Category = "Oranges"}//usando distinct(1)
            });
            
            //arrange
            NavController target = new NavController(mock.Object);

            //act - obtem o conjunto de categorias
            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();

            //asserts
            Assert.AreEqual(results.Length,3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] 
            {
                new Product {ProductID = 1, Name = "p1" ,Category = "Apples"},
                new Product {ProductID = 2, Name = "p2",Category = "Oranges"}
            });

            //arrange - criando a controller
            NavController target = new NavController(mock.Object);

            //arrange - define a categoria selecionada
            string categoryToSelect = "Apples";

            //act
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            //assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            //arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] 
            {
                   new Product {ProductID = 1, Name = "p1" ,Category = "Cat1"},
                   new Product {ProductID = 2, Name = "p2",Category = "Cat2"},
                   new Product {ProductID = 3, Name = "p3",Category = "Cat1"},
                   new Product {ProductID = 4, Name = "p4",Category = "Cat2"},
                   new Product {ProductID = 5, Name = "p5",Category = "Cat3"}
            });

            //arrange - criando a controller
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            //act - testa a quantidade de produtos por categoria.
            int res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            //assert
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2,2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
