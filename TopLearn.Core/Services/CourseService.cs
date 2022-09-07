using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;
using TopLearn.DataLayer.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using TopLearn.Core.Generators;
using TopLearn.Core.DTOs.Course;
using TopLearn.Core.Convertors;
using TopLearn.Core.Security;

namespace TopLearn.Core.Services
{
    public class CourseService : ICourseService
    {
        private TopLearnContext _context;
        public CourseService(TopLearnContext context)
        {
            _context = context;
        }

        public void AddComment(CourseComment comment)
        {
            _context.CourseComments.Add(comment);
            _context.SaveChanges();
        }

        public int AddCourse(Course course, IFormFile imageUp, IFormFile demoUp)
        {
            course.CreateDate = DateTime.Now;
            course.CourseImageName = "no-photo.jpg";

            if (imageUp != null && imageUp.IsImage())
            {
                course.CourseImageName = NameGenerator.GenerateUniqCode() + Path.GetExtension(imageUp.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/image", course.CourseImageName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    imageUp.CopyTo(stream);
                }

                //resize image to thumb
                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb", course.CourseImageName);

                ImageConvertor resizeImg = new ImageConvertor();
                resizeImg.Image_resize(imagePath, thumbPath, 150);
            }

            if (demoUp != null)
            {
                course.DemoFileName = NameGenerator.GenerateUniqCode() + Path.GetExtension(demoUp.FileName);
                string demoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/Demoes", course.DemoFileName);
                using (var stream = new FileStream(demoPath, FileMode.Create))
                {
                    demoUp.CopyTo(stream);
                }
            }
            else
            {
                course.DemoFileName = "no-photo.jpg";
            }
            _context.Add(course);
            _context.SaveChanges();

            return course.CourseId;
        }

        public int AddEpisode(CourseEpisode episode, IFormFile file)
        {
            episode.EpisodeFileName = file.FileName;

            if (file != null)
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseFile", episode.EpisodeFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            _context.Add(episode);
            _context.SaveChanges();

            return episode.EpisodeId;
        }

        public void AddUserCourse(int userId, int courseId)
        {
            _context.UserCourses.Add(new UserCourse
            {
                CourseId = courseId,
                UserId = userId
            });
            _context.SaveChanges();
        }

        public bool CheckFileExist(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseFile", fileName);
            return File.Exists(filePath);
        }

        public Tuple<List<CourseComment>, int> GetAllComment(int courseId, int pagId)
        {

            int take = 5;
            int skip = (pagId - 1) * take;
            int count = _context.CourseComments.Where(cc => cc.CourseId == courseId).Count()/take+1;

            return Tuple.Create(_context.CourseComments
                .Include(c=>c.User).Where(cc => cc.CourseId == courseId)
                .OrderByDescending(c=>c.dateTime)
                .Skip(skip).Take(take).ToList(), count);
        }

        public List<CourseGroup> GetAllCourses()
        {
            return _context.CourseGroups.ToList();
        }

        public List<CourseEpisode> GetAllEpisodesByCourseId(int courseId)
        {
            return _context.CourseEpisodes.Where(e => e.CourseId == courseId).ToList();
        }

        public List<SelectListItem> GetBigCourseGroups()
        {
            return _context.CourseGroups.Where(g => g.ParentId == null)
                .Select(g => new SelectListItem()
                {
                    Text = g.GroupTitle,
                    Value = g.GroupId.ToString()
                }).ToList();
        }

        public Course GetCourseById(int id)
        {
            return _context.Courses.Find(id);
        }

        public Course GetCourseForShow(int courseId)
        {
            return _context.Courses.Include(c => c.CourseEpisodes).Include(c => c.CourseLevel)
                .Include(c => c.CourseStatus).Include(c => c.User)
                .Include(c=>c.UserCourses)
                .FirstOrDefault(c => c.CourseId == courseId);
        }

        public List<ShowCourseForAdminViewModel> GetCourseForShowForAdmin()
        {
            return _context.Courses.Select(c => new ShowCourseForAdminViewModel()
            {
                CourseId = c.CourseId,
                ImageName = c.CourseImageName,
                TeacherName = c.User.UserName,
                Title = c.CourseTitle,
                EpisodeCount = c.CourseEpisodes.Count()
            }).ToList();
        }

        public Tuple<List<ShowCourseItemListViewModel>, int> GetCourseItemListViewModels(int pageId = 1
            , string filter = "", string getType = "all", string orderByType = "date"
            , int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null, int take = 0)
        {

            IQueryable<Course> result = _context.Courses;

            if (!string.IsNullOrEmpty(filter))
            {
                result = result.Where(c => c.CourseTitle.Contains(filter) || c.Tags.Contains(filter));
            }

            switch (getType)
            {
                case "all":
                    break;
                case "buy":
                    {
                        result = result.Where(c => c.CoursePrice != 0);
                        break;
                    }
                case "free":
                    {
                        result = result.Where(c => c.CoursePrice == 0);
                        break;
                    }
            }

            switch (orderByType)
            {
                case "date":
                    {
                        result = result.OrderByDescending(c => c.CreateDate);
                        break;
                    }
                case "updatedate":
                    {
                        result = result.OrderByDescending(c => c.UpdateDate);
                        break;
                    }
            }

            if (startPrice > 0)
            {
                result = result.Where(c => c.CoursePrice > startPrice);
            }
            if (endPrice > 0)
            {
                result = result.Where(c => c.CoursePrice < endPrice);
            }

            if (selectedGroups != null && selectedGroups.Any())
            {
                foreach (var groupId in selectedGroups)
                {
                    result = result.Where(c => c.GroupId == groupId || c.SubGroup == groupId);
                }
            }

            if (take == 0)
                take = 8;

            int skip = (pageId - 1) * take;

            int pageCount = result.Include(c => c.CourseEpisodes).AsEnumerable().Select(c => new ShowCourseItemListViewModel()
            {
                CourseId = c.CourseId,
                ImageName = c.CourseImageName,
                Price = c.CoursePrice,
                Title = c.CourseTitle,
                TotalTime = new TimeSpan(c.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks))
            }).Count() / take;

            List<ShowCourseItemListViewModel> query = result.Include(c => c.CourseEpisodes).AsEnumerable().Select(c => new ShowCourseItemListViewModel()
            {
                CourseId = c.CourseId,
                ImageName = c.CourseImageName,
                Price = c.CoursePrice,
                Title = c.CourseTitle,
                TotalTime = new TimeSpan(c.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks))
            }).Skip(skip).Take(take).ToList();

            return Tuple.Create(query, pageCount);

        }

        public List<SelectListItem> GetCourseLevels()
        {
            return _context.CourseLevels.Select(l => new SelectListItem()
            {
                Value = l.LevelId.ToString(),
                Text = l.LevelTitle
            }).ToList();
        }

        public List<SelectListItem> GetCourseStatuses()
        {
            return _context.CourseStatuses.Select(s => new SelectListItem()
            {
                Value = s.StatusId.ToString(),
                Text = s.StatusTitle
            }).ToList();
        }

        public List<SelectListItem> GetCourseTeachers()
        {
            return _context.UserRoles.Where(u => u.RoleId == 2)
                .Select(u => new SelectListItem()
                {
                    Value = u.UserId.ToString(),
                    Text = u.User.UserName
                }).ToList();
        }

        public CourseEpisode GetEpisodeById(int id)
        {
            return _context.CourseEpisodes.Find(id);
        }

        public List<SelectListItem> GetSmallCourseGroups(int bigGroupId)
        {
            return _context.CourseGroups.Where(g => g.ParentId == bigGroupId)
                .Select(g => new SelectListItem()
                {
                    Text = g.GroupTitle,
                    Value = g.GroupId.ToString()
                }).ToList();
        }

        public void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseDemo)
        {
            course.UpdateDate = DateTime.Now;

            if (imgCourse != null && imgCourse.IsImage())
            {
                if (course.CourseImageName != "no-photo.jpg")
                {
                    string deleteimagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/image", course.CourseImageName);
                    if (File.Exists(deleteimagePath))
                    {
                        File.Delete(deleteimagePath);
                    }

                    string deletethumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb", course.CourseImageName);
                    if (File.Exists(deletethumbPath))
                    {
                        File.Delete(deletethumbPath);
                    }
                }
                course.CourseImageName = NameGenerator.GenerateUniqCode() + Path.GetExtension(imgCourse.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/image", course.CourseImageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }

                ImageConvertor imgResizer = new ImageConvertor();
                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb", course.CourseImageName);

                imgResizer.Image_resize(imagePath, thumbPath, 150);
            }

            if (courseDemo != null)
            {
                if (course.DemoFileName != null)
                {
                    string deleteDemoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/demoes", course.DemoFileName);
                    if (File.Exists(deleteDemoPath))
                    {
                        File.Delete(deleteDemoPath);
                    }
                }
                course.DemoFileName = NameGenerator.GenerateUniqCode() + Path.GetExtension(courseDemo.FileName);
                string demoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/demoes", course.DemoFileName);
                using (var stream = new FileStream(demoPath, FileMode.Create))
                {
                    courseDemo.CopyTo(stream);
                }
            }

            _context.Courses.Update(course);
            _context.SaveChanges();
        }

        public void UpdateEpisode(CourseEpisode episode, IFormFile file)
        {
            if (file != null)
            {
                string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseFile", episode.EpisodeFileName);
                File.Delete(deletePath);
                episode.EpisodeFileName = file.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseFile", episode.EpisodeFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

            }

            _context.Update(episode);
            _context.SaveChanges();
        }
    }
}
