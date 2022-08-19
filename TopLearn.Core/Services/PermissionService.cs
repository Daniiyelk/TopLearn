using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Permission;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;


namespace TopLearn.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private TopLearnContext _context;
        public PermissionService(TopLearnContext context)
        {
            _context = context;
        }

        public void AddRolePermissions(int roleId, List<int> seledtedPermission)
        {
            foreach (var item in seledtedPermission)
            {
                _context.RolePermissions.Add(new RolePermission()
                {
                    PermissionId = item,
                    RoleId = roleId
                });
            }

            _context.SaveChanges();
        }

        public bool CheckPermission(int permissionId, string userName)
        {
            int userId = _context.Users.Single(u => u.UserName == userName).UserId;

            List<int> userRoles = _context.UserRoles
                .Where(r => r.UserId == userId).Select(r => r.RoleId).ToList();

            if (!userRoles.Any())
                return false;

            List<int> permissionRole = _context.RolePermissions
                .Where(r => r.PermissionId == permissionId).Select(r => r.RoleId).ToList();

            return permissionRole.Any(r => userRoles.Contains(r));
        }

        public List<Permission> GetAllPermissions()
        {
            return _context.Permissions.ToList();
        }

        public List<int> GetRolePermissions(int roleId)
        {
            return _context.RolePermissions.Where(r => r.RoleId == roleId).Select(p => p.PermissionId).ToList();
        }

        public void UpdateRolePermissions(int roleId, List<int> seledtedPermission)
        {
            _context.RolePermissions.Where(r => r.RoleId == roleId).ToList().ForEach(p => _context.RolePermissions.Remove(p));

            AddRolePermissions(roleId, seledtedPermission);
        }
    }
}
