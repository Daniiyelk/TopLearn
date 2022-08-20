using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopLearn.DataLayer.Entities.Order
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        [Required]
        public int OrderSum { get; set; }
        public bool IsFinally { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }



        [ForeignKey("UserId")]
        public virtual User.User User { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
    }
}
