using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Core.Convertors;
using TopLearn.DataLayer.Entities.User;
using TopLearn.Core.Generators;
using TopLearn.Core.Security;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using TopLearn.Core.Senders;
using Microsoft.AspNetCore.Authorization;

namespace TopLearn.Web.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _userService;
        private IViewRenderService _viewRenderService;
        public AccountController(IUserService userService, IViewRenderService viewRenderService)
        {
            _userService = userService;
            _viewRenderService = viewRenderService;
        }

        #region Register

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            if (_userService.IsExistUserName(register.UserName))
            {
                ModelState.AddModelError("UserName", "نام کاربری معتبر نمی باشد");
                return View(register);
            }

            if (_userService.IsExistEmail(FixedText.FixedEmail(register.Email)))
            {
                ModelState.AddModelError("Email", "ایمیل معتبر نمی باشد");
                return View(register);
            }

            var user = new User()
            {
                UserName = register.UserName,
                Email = FixedText.FixedEmail(register.Email),
                Password = PasswordHelper.EncodePasswordMd5(register.Password),
                ActiveCode = NameGenerator.GenerateUniqCode(),
                IsActive = false,
                RegisterDate = DateTime.Now,
                UserAvatar = "Defult.jpg",
            };

            int UserId = _userService.AddUser(user);

            #region Send Email Activation
            string body = _viewRenderService.RenderToStringAsync("_ActiveEmail", user);
            SendEmail.Send(user.Email, "فعالسازی حساب کاربری", body);
            #endregion

            return View("SuccessRegister", user);
        }

        #endregion

        #region LogIn

        [Route("Login")]
        public IActionResult LogIn(bool EditProfile = false)
        {
            ViewBag.EditProfile = EditProfile;
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult LogIn(LogInViewModel logIn)
        {
            if (!ModelState.IsValid)
            {
                return View(logIn);
            }

            var user = _userService.LogInUser(logIn);

            if (user != null)
            {
                if (user.IsActive)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                        new Claim(ClaimTypes.Name,user.UserName)
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = logIn.RememberMe
                    };
                    HttpContext.SignInAsync(principal, properties);
                    ViewBag.IsSuccess = true;
                    return View();
                }
                else
                {
                    ModelState.AddModelError("Password", "حساب کاربری شما فعال نمی باشد");
                    return View(logIn);
                }

            }

            ModelState.AddModelError("Password", "کاربری با مشخصات وارد شده یافت نشد");
            return View(logIn);
        }

        #endregion

        #region LogOut
        [Route("LogOut")]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/LogIn");
        }
        #endregion

        #region ActiveAccount

        public IActionResult ActiveAccount(string id)
        {
            ViewBag.IsActive = _userService.ActiveAccount(id);
            return View();
        }

        #endregion

        #region Forgot Password
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword(ForgotPasswordViewModel forgot)
        {
            if (!ModelState.IsValid)
            {
                return View(forgot);
            }

            var user = _userService.GetUserByEmail(FixedText.FixedEmail(forgot.Email));

            if (user == null)
            {
                ModelState.AddModelError("Email", "کاربری با ایمیل زیر یافت نشد");
                return View(forgot);
            }

            string emailBody = _viewRenderService.RenderToStringAsync("_ForgotPasswordEmail", forgot);
            SendEmail.Send(user.Email, "بازیابی کلمه عبور", emailBody);

            return View();
        }

        #endregion

        #region Reset Password

        public IActionResult ResetPassword(string id)
        {
            return View(new ResetPasswordViewModel()
            {
                ActiveCode = id
            });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel reset)
        {
            if (!ModelState.IsValid)
            {
                return View(reset); 
            }

            var user = _userService.GetUserByActiveCode(reset.ActiveCode);
            if (user == null)
            {
                return NotFound();
            }

            var hashedPassword = PasswordHelper.EncodePasswordMd5(reset.Password);
            user.Password = hashedPassword;
            _userService.UpdateUser(user);
            ViewBag.IsSuccess = true;

            return Redirect("/LogIn");
        }
        #endregion

    }
}
