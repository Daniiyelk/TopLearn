using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.DTOs.Course;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Core.Services.Interfaces
{
    public interface ICourseService
    {
        List<CourseGroup> GetAllCourses();
        List<SelectListItem> GetBigCourseGroups();
        List<SelectListItem> GetSmallCourseGroups(int bigGroupId);
        List<SelectListItem> GetCourseTeachers();
        List<SelectListItem> GetCourseLevels();
        List<SelectListItem> GetCourseStatuses();

        #region Course
        int AddCourse(Course course,IFormFile imageUp,IFormFile demoUp);
        void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseDemo);
        List<ShowCourseForAdminViewModel> GetCourseForShowForAdmin();
        Tuple<List<ShowCourseItemListViewModel>, int> GetCourseItemListViewModels(int pageId=1,string filter = ""
            ,string getType = "all",string orderByType = "date",int startPrice = 0,int endPrice=0
            ,List<int> selectedGroups = null,int take=0);
        Course GetCourseById(int id);
        Course GetCourseForShow(int courseId);
        void AddUserCourse(int userId, int courseId);

        #endregion

        #region Episode
        public int AddEpisode(CourseEpisode episode, IFormFile file);
        void UpdateEpisode(CourseEpisode episode, IFormFile file);
        bool CheckFileExist(string fileName);

        List<CourseEpisode> GetAllEpisodesByCourseId(int courseId);
        CourseEpisode GetEpisodeById(int id);
        #endregion

    }
}
