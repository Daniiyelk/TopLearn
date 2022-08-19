using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopLearn.Core.DTOs.Course
{
    public class ShowCourseItemListViewModel
    {
        public int CourseId { get; set; }
        public int Price { get; set; }
        public string Title { get; set; }
        public string ImageName { get; set; }
        public TimeSpan TotalTime { get; set; }

    }
}
