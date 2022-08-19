using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.Users
{
    [PermissionChecker(4)]
    public class EditUserModel : PageModel
    {
        private IUserService _userService;

        public EditUserModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public EditUserByAdminViewModel editUserByAdminViewModel { get; set; }
        
        public void OnGet(int id)
        {
            editUserByAdminViewModel = _userService.GetEditUserByAdmin(id);
            ViewData["Roles"] = _userService.GetRole();
        }
        public IActionResult OnPost(List<int> SelectedRoles)
        {

            _userService.UpdateUserByAdmin(editUserByAdminViewModel);
            _userService.EditRoleUser(editUserByAdminViewModel.UserId, SelectedRoles);

            return RedirectToPage("Index");
        }
    }
}
