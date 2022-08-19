using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Admin.Roles
{
    [PermissionChecker(8)]
    public class EditRoleModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;

        public EditRoleModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        [BindProperty]
        public Role role { get; set; }

        public void OnGet(int id)
        {
            role = _userService.GetRoleById(id);
            ViewData["Permissions"] = _permissionService.GetAllPermissions();
            ViewData["SelectedPermission"] = _permissionService.GetRolePermissions(id);
        }

        public IActionResult OnPost(List<int> SelectedPermission)
        {
            _userService.UpdateRole(role);
            _permissionService.UpdateRolePermissions(role.RoleId, SelectedPermission);

            return RedirectToPage("Index");
        }
    }
}
