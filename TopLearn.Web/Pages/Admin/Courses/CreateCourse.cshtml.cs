using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Admin.Courses
{
    public class CreateCourseModel : PageModel
    {
        private ICourseService _courseService;
        public CreateCourseModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public Course Course { get; set; }

        public void OnGet()
        {
            var BigGroups = _courseService.GetBigCourseGroups();
            ViewData["BigGroups"] = new SelectList(BigGroups,"Value","Text");

            var SmallGroups = _courseService.GetSmallCourseGroups(int.Parse(BigGroups.First().Value));
            ViewData["SmallGroups"] = new SelectList(SmallGroups, "Value", "Text");

            var Teachers = _courseService.GetCourseTeachers();
            ViewData["Teachers"] = new SelectList(Teachers, "Value", "Text");

            var Levels = _courseService.GetCourseLevels();
            ViewData["Levels"] = new SelectList(Levels, "Value", "Text");

            var Statuses = _courseService.GetCourseStatuses();
            ViewData["Statuses"] = new SelectList(Statuses, "Value", "Text");
        }

        public IActionResult OnPost(IFormFile imgCourseUp, IFormFile demoUp)
        {

            _courseService.AddCourse(Course,imgCourseUp,demoUp);

            return RedirectToPage("Index");
        }
    }
}
