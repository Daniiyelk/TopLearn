using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Admin.Users
{
    [PermissionChecker(3)]
    public class CreateUserModel : PageModel
    {
        private IUserService _userService;
        public CreateUserModel(IUserService userService)
        {
            _userService = userService;
         }

        [BindProperty]
        public CreateUserByAdminViewModel createUserByAdminViewModel { get; set; }
        public void OnGet()
        {
            ViewData["Roles"] = _userService.GetRole();
        }
        public IActionResult OnPost(List<int> SelectedRoles)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_userService.IsExistUserName(createUserByAdminViewModel.UserName))
            {
                ModelState.AddModelError("UserName", "نام کاربری معتبر نمی باشد");
                return Page();
            }

            if (_userService.IsExistEmail(FixedText.FixedEmail(createUserByAdminViewModel.Email)))
            {
                ModelState.AddModelError("Email", "ایمیل معتبر نمی باشد");
                return Page();
            }

            int userId = _userService.CreateUserByAdmin(createUserByAdminViewModel);
            _userService.AddRolesToUser(SelectedRoles, userId);

            return Redirect("/admin/users");
        }
    }
}
