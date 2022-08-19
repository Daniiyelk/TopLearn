using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Admin.Roles
{
    [PermissionChecker(7)]
    public class CreateRole : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;

        public CreateRole(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        [BindProperty]
        public Role Role { get; set; }

        public void OnGet()
        {
            ViewData["Permissions"] = _permissionService.GetAllPermissions();
        }
        public IActionResult OnPost(List<int> SelectedPermission)
        {
            Role.IsDeleted = false;
            _userService.AddRole(Role);
            _permissionService.AddRolePermissions(Role.RoleId, SelectedPermission);

            return RedirectToPage("index");
        }
    }
}
