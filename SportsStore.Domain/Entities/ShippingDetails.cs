using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SportsStore.Domain.Entities
{
    public class ShippingDetails
    {
        [Required(ErrorMessage="Informe o nome")]
        public string Name { get; set; }

        [Required(ErrorMessage="Informe ao menos um endereço")]
        [Display(Name="Line 1")]
        public string Line1 { get; set; }
        [Display(Name="Line 2")]
        public string Line2 { get; set; }
        [Display(Name="Line 3")]
        public string Line3 { get; set; }
        
        [Required(ErrorMessage="Informe o nome da cidade")]
        public string City { get; set; }

        [Required(ErrorMessage="Informe o estado")]
        public string State { get; set; }

        public string Zip { get; set; }

        [Required(ErrorMessage = "Informe o nome país")]
        public string Country { get; set; }

        public bool GiftWrap { get; set; }
    }
}
