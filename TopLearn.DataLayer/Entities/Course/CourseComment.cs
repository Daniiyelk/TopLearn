using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopLearn.DataLayer.Entities.Course
{
    public class CourseComment
    {
        [Key]
        public int CCommentId { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        [MaxLength(700)]
        public string Comment { get; set; }
        public DateTime dateTime { get; set; }
        public bool IsDelete { get; set; }
        public bool IsReadAdmin { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        [ForeignKey("UserId")]
        public User.User User { get; set; }
    }
}
