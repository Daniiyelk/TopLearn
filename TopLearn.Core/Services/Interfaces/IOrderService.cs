using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.DataLayer.Entities.Order;
using TopLearn.Core.DTOs;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IOrderService
    {
        bool IsFinallyOrder(string userName, int orderId);
        int AddOrder(string userName, int courseId);
        void CalculateOrderSum(int orderId);
        void UpdateOrder(Order order);
        Order ShowOrderForUserPanel(string userName, int orderId);
        Order GetOrderById(int id);
        List<Order> GetAllOrders(string userName);

        #region Discount
        DiscountEnumReturn UseDiscount(int orderId,string code);
        List<Discount> GetAllDiscount();
        Discount GetDiscountById(int id);   
        void UpdateDiscount(Discount discount);
        void AddDiscount(Discount discount);
        bool IsExistCode(string code);
        #endregion
    }
}
