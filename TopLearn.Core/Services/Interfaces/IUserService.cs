using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.DTOs;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IUserService
    {
        #region User Panel
        bool IsExistUserName(string userName);
        bool IsExistEmail(string email);
        bool ActiveAccount(string activeCode);
        int AddUser(User user);
        int GetUserIdByUserName(string userName);
        User LogInUser(LogInViewModel logIn);
        User GetUserByEmail(string email);
        User GetUserByActiveCode(string activeCode);
        User GetUserByUserName(string userName);
        User GetUserByUserId(int userId);
        void UpdateUser(User user);

        
        InformationUserViewModel GetUserInformation(string userName);
        InformationUserViewModel GetUserInformation(int userId);
        SideBarUserPanelViewModel GetSideBarUserPanelData(string userName);
        EditProfileViewModel GetDataEditProfile(string userName);
        List<ChargeWalletHistoryViewModel> GetDataChargeWalletHistory(string userName);
        Wallet GetWalletByWalletId(int walletId);
        void EditProfile(string userName,EditProfileViewModel profile);
        void ChangeUserPassword(string userName,string password);
        void AddWallet(Wallet wallet);
        void AddUserDiscountCode(int userId, int discountId);
        void UpdateWallet(Wallet wallet);
        bool CompareUserPasswordForChange(string userName,string oldPassword);
        int BalanceUserWallet(string userName);
        int ChargeWallet(string userName, int amount, string desc, bool isPay = false);
        #endregion

        #region Admin Panel

        #region Users
        UserViewViewModel GetUsers(int pagId = 1, string filterEmail = "", string filterUserName = "");
        public UserViewViewModel GetDletedUsers(int pageId = 1, string filterEmail = "", string filterUserName = "");
        EditUserByAdminViewModel GetEditUserByAdmin(int userId);
        List<Role> GetRole();
        int CreateUserByAdmin(CreateUserByAdminViewModel model);
        void AddRolesToUser(List<int> roles, int userId);
        void UpdateUserByAdmin(EditUserByAdminViewModel model);
        void EditRoleUser(int userId, List<int> roles);
        void DeleteUserByAdmin(int userId);
        #endregion
        
        #region Role
        List<Role> GetRoles();
        Role GetRoleById(int id);   
        void UpdateRole(Role role); 
        void DeleteRole(Role role);
        void AddRole(Role role);
        #endregion

        #endregion

    }
}
