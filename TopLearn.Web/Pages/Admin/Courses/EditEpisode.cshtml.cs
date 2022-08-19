using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Admin.Courses
{
    public class EditEpisodeModel : PageModel
    {

        private ICourseService _courseService;
        public EditEpisodeModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public CourseEpisode CourseEpisode { get; set; }

        public void OnGet(int id)
        {
            CourseEpisode = _courseService.GetEpisodeById(id); 
        }

        public IActionResult OnPost(IFormFile FileEpisode)
        {
            if (FileEpisode != null && _courseService.CheckFileExist(FileEpisode.FileName))
            {
                ViewData["IsFileExist"] = true;
                return Page();
            }

            _courseService.UpdateEpisode(CourseEpisode, FileEpisode);

            return Redirect("/Admin/Courses/IndexEpisode/" + CourseEpisode.CourseId.ToString());
        }
    }
}
