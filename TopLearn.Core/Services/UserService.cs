using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Generators;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Core.Services
{
    public class UserService : IUserService
    {
        private TopLearnContext _context;
        public UserService(TopLearnContext context)
        {
            _context = context;
        }

        public int AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.UserId;
        }

        public bool ActiveAccount(string activeCode)
        {
            var user = _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
            if (user == null || user.IsActive)
            {
                return false;
            }
            user.IsActive = true;
            user.ActiveCode = NameGenerator.GenerateUniqCode();
            _context.SaveChanges();
            return true;
        }

        public bool IsExistEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public bool IsExistUserName(string userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }

        public User LogInUser(LogInViewModel logIn)
        {
            var email = FixedText.FixedEmail(logIn.Email);
            var hashedpassword = PasswordHelper.EncodePasswordMd5(logIn.Password);

            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == hashedpassword);
            return user;
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email);
        }

        public User GetUserByActiveCode(string activeCode)
        {
            return _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
        }

        public void UpdateUser(User user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }

        public User GetUserByUserName(string userName)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == userName);
        }

        InformationUserViewModel IUserService.GetUserInformation(string userName)
        {
            var user = GetUserByUserName(userName);
            InformationUserViewModel model = new InformationUserViewModel()
            {
                UserName = user.UserName,
                Email = user.Email,
                RegisterDate = user.RegisterDate,
                Wallet = BalanceUserWallet(userName),
            };
            return model;
        }

        public SideBarUserPanelViewModel GetSideBarUserPanelData(string userName)
        {
            var user = GetUserByUserName(userName);
            SideBarUserPanelViewModel sideBarUser = new SideBarUserPanelViewModel()
            {
                ImageName = user.UserAvatar,
                RegisterDate = user.RegisterDate,
                UserName = user.UserName,
            };
            return sideBarUser;
        }

        public EditProfileViewModel GetDataEditProfile(string userName)
        {
            var user = GetUserByUserName(userName);
            EditProfileViewModel model = new EditProfileViewModel()
            {
                Email = user.Email,
                UserName = user.UserName,
                AvatarName = user.UserAvatar,
            };
            return model;
        }

        public void EditProfile(string userName, EditProfileViewModel profile)
        {

            if (profile.UserAvatar != null)
            {
                string imagePath = "";
                if (profile.AvatarName != "Defualt.jpg")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", profile.AvatarName);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }
                profile.AvatarName = NameGenerator.GenerateUniqCode + Path.GetExtension(profile.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", profile.AvatarName);

                using (FileStream fs = new FileStream(imagePath, FileMode.Create))
                {
                    profile.UserAvatar.CopyTo(fs);
                }

            }

            var user = GetUserByUserName(userName);
            user.UserAvatar = profile.AvatarName;
            user.UserName = profile.UserName;
            user.Email = profile.Email;

            UpdateUser(user);
        }

        public bool CompareUserPasswordForChange(string userName, string oldPassword)
        {
            oldPassword = PasswordHelper.EncodePasswordMd5(oldPassword);
            return _context.Users.Any(u => u.UserName == userName && u.Password == oldPassword);
        }

        public void ChangeUserPassword(string userName, string password)
        {
            var user = GetUserByUserName(userName);
            user.Password = PasswordHelper.EncodePasswordMd5(password);
            UpdateUser(user);
        }

        public int BalanceUserWallet(string userName)
        {
            int userId = GetUserIdByUserName(userName);
            var enter = _context.Wallets.Where(w => w.UserId == userId && w.TypeId == 1 && w.IsPay == true)
                .Select(w => w.Amount).ToList();

            var exit = _context.Wallets.Where(w => w.UserId == userId && w.TypeId == 2 && w.IsPay == true)
                .Select(w => w.Amount).ToList();

            return (enter.Sum() - exit.Sum());
        }

        public int GetUserIdByUserName(string userName)
        {
            return _context.Users.Single(u => u.UserName == userName).UserId;
        }

        public List<ChargeWalletHistoryViewModel> GetDataChargeWalletHistory(string userName)
        {
            int userId = GetUserIdByUserName(userName);
            return _context.Wallets
                .Where(w => w.UserId == userId && w.IsPay == true)
                .Select(w => new ChargeWalletHistoryViewModel()
                {
                    Amount = w.Amount,
                    DateTime = w.CreateDate,
                    Description = w.Description,
                    Type = w.TypeId,
                })
                .ToList();
        }

        public int ChargeWallet(string userName, int amount, string desc, bool isPay = false)
        {
            Wallet wallet = new Wallet()
            {
                Amount = amount,
                CreateDate = DateTime.Now,
                Description = desc,
                IsPay = isPay,
                TypeId = 1,
                UserId = GetUserIdByUserName(userName),

            };

            AddWallet(wallet);
            return wallet.WalletId;
        }
        public void AddWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
        }

        public Wallet GetWalletByWalletId(int walletId)
        {
            return _context.Wallets.Find(walletId);
        }

        public void UpdateWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }

        public UserViewViewModel GetUsers(int pageId = 1, string filterEmail = "", string filterUserName = "")
        {
            IQueryable<User> users = _context.Users;

            if (!string.IsNullOrEmpty(filterEmail))
            {
                users = users.Where(u => u.Email.Contains(filterEmail));
            }

            if (!string.IsNullOrEmpty(filterUserName))
            {
                users = users.Where(u => u.UserName.Contains(filterUserName));
            }

            var take = 20;
            var skip = (pageId - 1) * take;

            UserViewViewModel list = new UserViewViewModel()
            {
                CurrentPage = pageId,
                PageCount = users.Count() / take,
                Users = users.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList(),
            };

            return list;
        }

        public UserViewViewModel GetDletedUsers(int pageId = 1, string filterEmail = "", string filterUserName = "")
        {
            IQueryable<User> users = _context.Users.IgnoreQueryFilters().Where(u => u.IsDeleted);

            if (!string.IsNullOrEmpty(filterEmail))
            {
                users = users.Where(u => u.Email.Contains(filterEmail));
            }

            if (!string.IsNullOrEmpty(filterUserName))
            {
                users = users.Where(u => u.UserName.Contains(filterUserName));
            }

            var take = 20;
            var skip = (pageId - 1) * take;

            UserViewViewModel list = new UserViewViewModel()
            {
                CurrentPage = pageId,
                PageCount = users.Count() / take,
                Users = users.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList(),
            };

            return list;
        }

        public List<Role> GetRole()
        {
            return _context.Roles.ToList();
        }

        public int CreateUserByAdmin(CreateUserByAdminViewModel model)
        {

            var user = new User()
            {
                Email = FixedText.FixedEmail(model.Email),
                UserName = model.UserName,
                ActiveCode = NameGenerator.GenerateUniqCode(),
                Password = PasswordHelper.EncodePasswordMd5(model.Password),
                IsActive = true,
                RegisterDate = DateTime.Now,
            };

            if (model.UserAvatar != null)
            {
                string imagePath = "";
                user.UserAvatar = NameGenerator.GenerateUniqCode + Path.GetExtension(model.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar",user.UserAvatar);

                using (FileStream fs = new FileStream(imagePath, FileMode.Create))
                {
                    model.UserAvatar.CopyTo(fs);
                }
            }

            return AddUser(user);
        }

        public void AddRolesToUser(List<int> roles, int userId)
        {
            foreach (var role in roles)
            {
                _context.UserRoles.Add(new UserRole()
                {
                    RoleId = role,
                    UserId = userId
                });
            }
            _context.SaveChanges();
        }

        public EditUserByAdminViewModel GetEditUserByAdmin(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId).Select(u => new EditUserByAdminViewModel()
            {
                UserId = userId,
                Email = u.Email,
                UserName = u.UserName,
                UserAvataName = u.UserAvatar,
                UserRoles = u.UserRoles.Select(w=>w.RoleId).ToList(),
            }).Single();
        }

        public void UpdateUserByAdmin(EditUserByAdminViewModel model)
        {
            var user = GetUserByUserId(model.UserId);

            user.Email = FixedText.FixedEmail(model.Email);
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = PasswordHelper.EncodePasswordMd5(model.Password);
            }

            if (model.UserAvatar != null)
            {
                //Delete old Image
                if (model.UserAvataName != "Defult.jpg")
                {
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", model.UserAvataName);
                    if (File.Exists(deletePath))
                    {
                        File.Delete(deletePath);
                    }
                }

                //Save New Image
                user.UserAvatar = NameGenerator.GenerateUniqCode() + Path.GetExtension(model.UserAvatar.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", user.UserAvatar);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    model.UserAvatar.CopyTo(stream);
                }
            }

            UpdateUser(user);
        }

        public User GetUserByUserId(int userId)
        {
            return _context.Users.Find(userId);
        }

        public void EditRoleUser(int userId, List<int> roles)
        {
            //remove previews Roles
            _context.UserRoles.Where(u => u.UserId == userId).ToList().ForEach(r => _context.UserRoles.Remove(r));

            AddRolesToUser(roles,userId);

        }

        public InformationUserViewModel GetUserInformation(int userId)
        {
            var user = GetUserByUserId(userId);
            InformationUserViewModel model = new InformationUserViewModel()
            {
                UserName = user.UserName,
                Email = user.Email,
                RegisterDate = user.RegisterDate,
                Wallet = BalanceUserWallet(user.UserName),
            };
            return model;
        }

        public void DeleteUserByAdmin(int userId)
        {
            var user = GetUserByUserId(userId);
            user.IsDeleted = true;
            UpdateUser(user);
        }

        public List<Role> GetRoles()
        {
            return _context.Roles.ToList();
        }

        public Role GetRoleById(int id)
        {
            return _context.Roles.Find(id);
        }

        public void UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
        }

        public void DeleteRole(Role role)
        {
            role.IsDeleted = true;
            UpdateRole(role);
        }

        public void AddRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
        }

    }
}
