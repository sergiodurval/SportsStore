using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace SportsStore.Domain.Entities
{
    public class Product
    {
        [HiddenInput(DisplayValue=false)]
        public int ProductID { get; set; }
        [Required(ErrorMessage="É necessário informar o nome do produto")]
        public string Name { get; set; }

        [Required(ErrorMessage="É necessário informar a descrição do produto")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [Range(0.01,double.MaxValue,ErrorMessage="Insira somente valores positivos")]
        public decimal Price { get; set; }

        [Required(ErrorMessage="É necessário informar a categoria do produto")]
        public string Category { get; set; }

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
    }
}
