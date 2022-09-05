using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces; 

namespace TopLearn.Web.Pages.Admin.Discount
{
    public class IndexModel : PageModel
    {
        private IOrderService _orderservice;
        public IndexModel(IOrderService orderservice)
        {
            _orderservice = orderservice;
        }

        public List<TopLearn.DataLayer.Entities.Order.Discount> Discounts { get; set; }

        public void OnGet()
        {
            Discounts = _orderservice.GetAllDiscount();
        }
    }
}
