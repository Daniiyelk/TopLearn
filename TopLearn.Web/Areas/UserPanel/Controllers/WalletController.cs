using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    [Authorize]
    [Area("UserPanel")]
    public class WalletController : Controller
    {
        private IUserService _userService;

        public WalletController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("UserPanel/ChargeWallet")]
        public IActionResult ChargeWallet()
        {
            ViewBag.ListDataChargeWalletHistory = _userService.GetDataChargeWalletHistory(User.Identity.Name);
            return View();
        }

        [Route("UserPanel/ChargeWallet")]
        [HttpPost]
        public IActionResult ChargeWallet(ChargeWalletViewModel charge)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ListDataChargeWalletHistory = _userService.GetDataChargeWalletHistory(User.Identity.Name);
                return View(charge);
            }

            int walletId = _userService.ChargeWallet(User.Identity.Name, charge.Amount, "شارژ کیف پول");

            #region Online Payment

            var payment = new ZarinpalSandbox.Payment(charge.Amount);

            var res = payment.PaymentRequest("شارژ کیف پول", "https://localhost:7042/OnlinePayment/" + walletId, "daniiyelkazemi666666@gmail.com", "09134485949");

            if (res.Result.Status == 100)
            {
                return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + res.Result.Authority);
            }

            #endregion

            return Redirect("/Userpanel");
        }
    }
}
