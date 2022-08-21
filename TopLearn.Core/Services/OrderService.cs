using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Core.Services
{
    public class OrderService : IOrderService
    {
        private TopLearnContext _context;
        private IUserService _userService;
        public OrderService(TopLearnContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public int AddOrder(string userName, int courseId)
        {
            int userId = _userService.GetUserIdByUserName(userName);
            var order = _context.Orders.FirstOrDefault(o => o.UserId == userId && !o.IsFinally);
            var course = _context.Courses.Find(courseId);

            if (order == null)
            {
                order = new Order
                {
                    IsFinally = false,
                    OrderSum = course.CoursePrice,
                    CreateDate = DateTime.Now,
                    UserId = userId,
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail
                        {
                             Count = 1,
                             CourseId = courseId,
                             Price = course.CoursePrice
                        }

                    }
                };
                _context.Orders.Add(order);
            }

            else
            {
                OrderDetail detail = _context.OrderDetails.FirstOrDefault(d=>d.CourseId==courseId && d.OrderId==order.OrderId);
                if(detail == null)
                {
                    detail = new OrderDetail
                    {
                        Count = 1,
                        CourseId = courseId,
                        OrderId = order.OrderId,
                        Price = course.CoursePrice,
                    };
                    _context.OrderDetails.Add(detail);
                }
                else
                {
                    detail.Count += 1;
                    _context.OrderDetails.Update(detail);
                }
                
            }

            _context.SaveChanges();
            CalculateOrderSum(order.OrderId);

            return order.OrderId;
        }

        public void CalculateOrderSum(int orderId)
        {
            Order order = _context.Orders.Find(orderId);
            order.OrderSum = _context.OrderDetails.Where(d => d.OrderId == order.OrderId)
                .Sum(d => d.Price * d.Count);

            _context.Update(order);
            _context.SaveChanges();
        }
    }
}
