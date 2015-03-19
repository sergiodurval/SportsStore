using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class ImageTests
    {
        [TestMethod]
        public void Can_Retrieve_Image_Data() //Pode recuperar os dados da imagem
        {
            //arrange - criando produto com imagem data.
            Product prod = new Product 
            {
                ProductID = 2,
               Name = "Test",
               ImageData = new byte[] { },
               ImageMimeType ="image/png"                                    
            };

            //arrange - criando mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                  new Product{ProductID = 1 , Name="P1"},prod,
                  new Product{ProductID = 3,Name="P3"}
            }.AsQueryable());

            //arrange - criando a controller
            ProductController target = new ProductController(mock.Object);

            //act - chama o método GetImage
            ActionResult result = target.GetImage(2);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(prod.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void Cannot_Retrieve_Image_Data_For_Invalid_ID()
        {
            //arrange - criando mock repository - Parei aqui
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] { 
                new Product{ProductID=1,Name="P1"},
                new Product{ProductID=2,Name="P2"}
            }.AsQueryable());

            //arrange - criando a controller
            ProductController target = new ProductController(mock.Object);

            //act - chama o método GetImage
            ActionResult result = target.GetImage(100);

            //assert
            Assert.IsNull(result);
        }
    }
}
