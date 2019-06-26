using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpWithEF.Models
{
    public class EditOrderModel
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string CarId { get; set; }

        public List<Car> Cars { get; set; }

        public EditOrderModel()
        {
            Cars = new List<Car>();
        }
    }
}
