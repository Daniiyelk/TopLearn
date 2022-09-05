using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.Discount
{
    public class CreateDiscountModel : PageModel
    {
        private IOrderService _orderservice;
        public CreateDiscountModel(IOrderService orderservice)
        {
            _orderservice = orderservice;
        }

        [BindProperty]
        public TopLearn.DataLayer.Entities.Order.Discount Discount { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string stDate,string edDate)
        {
            if(stDate != "")
            {
                string[] SplitedStDate=stDate.Split('/');
                Discount.StartDate = new DateTime(int.Parse(SplitedStDate[0]),
                    int.Parse(SplitedStDate[1]),
                    int.Parse(SplitedStDate[2]),
                    new PersianCalendar()
                );
            }

            if(edDate != "")
            {
                string[] SplitededDate = stDate.Split('/');
                Discount.EndDate = new DateTime(int.Parse(SplitededDate[0]),
                    int.Parse(SplitededDate[1]),
                    int.Parse(SplitededDate[2]),
                    new PersianCalendar()
                );
            }

            _orderservice.AddDiscount(Discount);

            return RedirectToPage("index");
        }
    }
}
