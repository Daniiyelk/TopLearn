using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.Discount
{
    public class EditDiscountModel : PageModel
    {
        private IOrderService _orderservice;
        public EditDiscountModel(IOrderService orderservice)
        {
            _orderservice = orderservice;
        }

        [BindProperty]
        public TopLearn.DataLayer.Entities.Order.Discount Discount { get; set; }

        public void OnGet(int id)
        {
            Discount = _orderservice.GetDiscountById(id);
        }

        public IActionResult OnPost()
        {
            _orderservice.UpdateDiscount(Discount);
            return RedirectToPage("index");
        }
    }
}
