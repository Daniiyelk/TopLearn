using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Admin.Roles
{
    [PermissionChecker(9)]
    public class DeleteRoleModel : PageModel
    {
        private IUserService _userService;

        public DeleteRoleModel(IUserService userService)
        {
            _userService = userService;
        }
        [BindProperty]
        public Role Role { get; set; }

        public void OnGet(int id)
        {
            Role = _userService.GetRoleById(id);

        }

        public IActionResult OnPost(int id)
        {
            _userService.DeleteRole(Role);

            return RedirectToAction("Index");
        }
    }
}
