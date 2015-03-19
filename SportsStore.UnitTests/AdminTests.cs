using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            //Arrange criando os produtos
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ProductID = 1 ,Name="P1"},
                new Product{ProductID = 2 ,Name="P2"},
                new Product{ProductID = 3 ,Name="P3"},
            });

            //Arrange criando a controller
            AdminController target = new AdminController(mock.Object);

            //Action
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

            //Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3",result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            //arrange criando os produtos
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ProductID = 1 ,Name="P1"},
                new Product{ProductID = 2,Name="P2" },
                new Product{ProductID = 3,Name="P3"},
            });

            //Arrange criando a controller
            AdminController target = new AdminController(mock.Object);

            //act
            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;

            //assert
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            //Realiza teste para evitar de editar um produto que não existe
            //Produtos
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ProductID = 1 ,Name="P1"},
                new Product{ProductID = 2,Name="P2" },
                new Product{ProductID = 3,Name="P3"},
            });

            //controller
            AdminController target = new AdminController(mock.Object);

            //act
            Product result = (Product)target.Edit(4).ViewData.Model;

            //assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            //salvando as alterações válidas.
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

            //controller
            AdminController target = new AdminController(mock.Object);

            //arrange - criando produto
            Product product = new Product { Name = "teste" };

            //act- tentando salvar o produto.
            ActionResult result = target.Edit(product);

            //act -verifica se o repositório foi chamado.
            mock.Verify(m => m.SaveProduct(product));

            //assert verifica o tipo do resultado do método.
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            //teste para validar se não esta sendo salvo valores inválidos.
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

            //arrange - criando o controller
            AdminController target = new AdminController(mock.Object);

            //arrange - criando o produto
            Product product = new Product {Name="teste" };

            //arrange - adicionado erro ao model state
            target.ModelState.AddModelError("error", "erro");

            //act - salvando os produtos
            ActionResult result = target.Edit(product);

            //assert - verificando se o repositório não foi chamado
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());

            //assert - verifica o tipo do resultado do método.
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            //arrange - criando produtos
            Product prod = new Product {Name="P2" };

            //create o mock repositório
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ProductID = 1 , Name="P1"},
                new Product{ProductID = 3 , Name="P3"},
            });

            //criando a controller
            AdminController target = new AdminController(mock.Object);

            //act - deletando o produto
            target.Delete(prod.ProductID);

            //assert - garantir que o repositorio deletou o produto correto.
            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }
    }
}
