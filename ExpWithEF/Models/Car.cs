using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ExpWithEF.Models
{
    public class Car
    {
        [Required(ErrorMessage = "Please enter a color")]
        public string color { get; set; }

        [Required(ErrorMessage = "Please enter a company")]
        public string company { get; set; }

        [Required(ErrorMessage = "Please enter a model")]
        public string model { get; set; }

        [Required(ErrorMessage = "Please enter a price")]
        public int price { get; set; }
        public string Id { get; set; }
    }
}
