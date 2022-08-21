using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    [Authorize]
    [Area("UserPanel")]
    public class OrderController : Controller
    {
        private IUserService _userService;
        private IOrderService _orderService;

        public OrderController(IUserService userService,IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("ShowOrder/{id}")]
        public IActionResult ShowOrder(int id)
        {
            var order = _orderService.ShowOrderForUserPanel(User.Identity.Name,id);

            return View(order);
        }
    }
}
