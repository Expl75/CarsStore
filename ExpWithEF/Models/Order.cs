using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpWithEF.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please, enter your name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please, enter your phone")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Please, enter your addres")]
        public string Address { get; set; }

        [ForeignKey("CarId")]
        public Car Car { get; set; }
    }
}
