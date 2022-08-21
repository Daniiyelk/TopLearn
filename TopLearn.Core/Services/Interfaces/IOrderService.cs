using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IOrderService
    {
        int AddOrder(string userName, int courseId);
        void CalculateOrderSum(int orderId);
        Order ShowOrderForUserPanel(string userName, int orderId);
    }
}
