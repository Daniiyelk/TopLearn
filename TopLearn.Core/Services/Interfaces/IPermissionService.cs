using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.DataLayer.Entities.Permission;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        #region Permission
        List<Permission> GetAllPermissions();
        List<int> GetRolePermissions(int roleId);
        void AddRolePermissions(int roleId,List<int> seledtedPermission);
        void UpdateRolePermissions(int roleId,List<int> seledtedPermission);
        bool CheckPermission(int permissionId, string userName);
        #endregion

    }
}
