using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Admin.Courses
{
    public class EditCourseModel : PageModel
    {
        private ICourseService _courseService;
        public EditCourseModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public Course Course { get; set; }

        public void OnGet(int id)
        {
            Course = _courseService.GetCourseById(id);

            var BigGroups = _courseService.GetBigCourseGroups();
            ViewData["BigGroups"] = new SelectList(BigGroups, "Value", "Text",Course.GroupId);

            var SmallGroups = _courseService.GetSmallCourseGroups(int.Parse(BigGroups.First().Value));
            ViewData["SmallGroups"] = new SelectList(SmallGroups, "Value", "Text", Course.SubGroup??0);

            var Teachers = _courseService.GetCourseTeachers();
            ViewData["Teachers"] = new SelectList(Teachers, "Value", "Text", Course.TeacherId);

            var Levels = _courseService.GetCourseLevels();
            ViewData["Levels"] = new SelectList(Levels, "Value", "Text", Course.LevelId);

            var Statuses = _courseService.GetCourseStatuses();
            ViewData["Statuses"] = new SelectList(Statuses, "Value", "Text", Course.StatusId);
        }

        public IActionResult OnPost(IFormFile imgCourseUp, IFormFile demoUp)
        {
            _courseService.UpdateCourse(Course, imgCourseUp, demoUp);

            return RedirectToPage("index");
        }
    }
}
