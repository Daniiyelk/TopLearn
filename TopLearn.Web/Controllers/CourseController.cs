using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Controllers
{
    public class CourseController : Controller
    {
        private ICourseService _courseService;
        private IOrderService _orderService;
        private IUserService _userService;

        public CourseController(ICourseService courseService, IOrderService orderService,IUserService userService)
        {
            _courseService = courseService;
            _orderService = orderService;
            _userService = userService;
        }

        public IActionResult Index(int pageId = 1, string filter = ""
            , string getType = "all", string orderByType = "date",
            int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null)
        {
            ViewBag.selectedGroups = selectedGroups;
            ViewBag.Groups = _courseService.GetAllCourses();
            ViewBag.pageId = pageId;

            return View(_courseService.GetCourseItemListViewModels(pageId, filter, getType, orderByType, startPrice, endPrice, selectedGroups, 9));
        }

        [Route("ShowCourse/{id}")]
        public IActionResult ShowCourse(int id)
        {
            var course = _courseService.GetCourseForShow(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [Authorize]
        [Route("BuyCourse/{id}")]
        public IActionResult BuyCourse(int id)
        {
            int orderId = _orderService.AddOrder(User.Identity.Name, id);

            return Redirect("/UserPanel/Order/ShowOrder/" + orderId);
        }

        [Authorize]
        [Route("DownloadFile/{episodeId}")]
        public IActionResult DownloadFile(int episodeId)
        {
            var episode = _courseService.GetEpisodeById(episodeId);
            
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseFile",episode.EpisodeFileName);
            string fileName = episode.EpisodeFileName;

            if (episode.IsFree)
            {
                byte[] file = System.IO.File.ReadAllBytes(filePath);
                return File(file,"application/force_download",fileName);
            }

            if (User.Identity.IsAuthenticated)
            {
                if (_orderService.IsUserInCourse(User.Identity.Name, episode.CourseId))
                {
                    byte[] file = System.IO.File.ReadAllBytes(filePath);
                    return File(file, "application/force_download", fileName);
                }
            }

            return Forbid();
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateComment(CourseComment comment)
        {
            comment.dateTime = DateTime.Now;
            comment.IsDelete = false;
            comment.IsReadAdmin = false;
            comment.UserId = _userService.GetUserIdByUserName(User.Identity.Name);
            _courseService.AddComment(comment);

            return View("ShowComment",_courseService.GetAllComment(comment.CourseId,1));
        }

        public IActionResult ShowComment(int id,int pageId=1)
        {
            return View(_courseService.GetAllComment(id,pageId));
        }
    }
}