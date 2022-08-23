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
            var orders = _orderService.GetAllOrders(User.Identity.Name);
            return View(orders);
        }

        public IActionResult ShowOrder(int id,bool finaly=false)
        {
            var order = _orderService.ShowOrderForUserPanel(User.Identity.Name,id);
            ViewBag.isFinally = finaly;

            return View(order);
        }

        public IActionResult FinallyOrder(int id)
        {
            var finaly = _orderService.IsFinallyOrder(User.Identity.Name,id);

            if (finaly)
            {
                return Redirect("/UserPanel/Order/ShowOrder/" + id + "/?finaly=true");
            }

            return BadRequest();
        }

        public IActionResult DisCount(int orderId,int code)
        {
            var output = _orderService.UseDiscount(orderId, code);

            return Redirect("/userpanel/order/showorder/"+orderId+"?type="+output.ToString());
        }
    }
}
