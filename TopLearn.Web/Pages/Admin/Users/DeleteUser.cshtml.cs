using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.Users
{
    [PermissionChecker(5)]
    public class DeleteUserModel : PageModel
    {
        private IUserService _userService;
        public DeleteUserModel(IUserService userService)
        {
            _userService = userService;
        }

        public InformationUserViewModel informationUserViewModel { get; set; }

        public void OnGet(int id)
        {
            @ViewData["UserId"] = id;
            informationUserViewModel = _userService.GetUserInformation(id);
        }
        
        public IActionResult OnPost(int id)
        {
            _userService.DeleteUserByAdmin(id);
            return RedirectToPage("DeletedUserList");
        }
    }
}
