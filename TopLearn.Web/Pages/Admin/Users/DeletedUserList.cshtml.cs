using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.Users
{
    [PermissionChecker(5)]
    public class DeletedUserListModel : PageModel
    {
        private IUserService _userService;
        public DeletedUserListModel(IUserService userService)
        {
            _userService = userService;
        }
        public UserViewViewModel userViewViewModel { get; set; }

        public void OnGet(int pageId = 1, string filterUserName = "", string filterEmail = "")
        {
            userViewViewModel = _userService.GetDletedUsers(pageId, filterEmail, filterUserName);
        }
    }
}
