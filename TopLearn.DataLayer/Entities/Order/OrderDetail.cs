﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopLearn.DataLayer.Entities.Order
{
    public class OrderDetail
    {
        [Key]
        public int DetailId { get; set; }
        public int OrderId { get; set; }
        public int CourseId { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public int Price { get; set; }


        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course.Course Course { get; set; }
    }
}
