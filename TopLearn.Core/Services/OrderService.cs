using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Core.Services
{
    public class OrderService : IOrderService
    {
        private TopLearnContext _context;
        private IUserService _userService;
        private ICourseService _courseService;
        public OrderService(TopLearnContext context, IUserService userService, ICourseService courseService)
        {
            _context = context;
            _userService = userService;
            _courseService = courseService;
        }

        public void AddDiscount(Discount discount)
        {
            _context.Add(discount);
            _context.SaveChanges();
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

        public List<Discount> GetAllDiscount()
        {
            return _context.Discounts.ToList();
        }

        public List<Order> GetAllOrders(string userName)
        {
            int userId = _userService.GetUserIdByUserName(userName);
            return _context.Orders.Where(o => o.UserId == userId).ToList();
        }

        public Discount GetDiscountById(int id)
        {
            return _context.Discounts.Find(id);
        }

        public Order GetOrderById(int id)
        {
            return _context.Orders.Find(id);
        }

        public bool IsFinallyOrder(string userName, int orderId)
        {
            int userId = _userService.GetUserIdByUserName(userName);
            var order = _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Course)
                .FirstOrDefault(o => o.UserId == userId && o.OrderId == orderId);
            if (order == null || order.IsFinally)
            {
                return false;
            }

            if (_userService.BalanceUserWallet(userName) >= order.OrderSum)
            {
                order.IsFinally = true;
                _userService.AddWallet(new DataLayer.Entities.Wallet.Wallet
                {
                    Amount = order.OrderSum,
                    CreateDate = DateTime.Now,
                    IsPay = true,
                    TypeId = 2,
                    UserId = userId,
                    Description = " پرداخت فاکتور شماره "+ order.OrderId,
                });

                _context.Update(order);
                _context.SaveChanges();

                foreach(var detail in order.OrderDetails)
                {
                    _courseService.AddUserCourse(userId, detail.CourseId);
                }

                return true;
            }

            return false;
        }

        public Order ShowOrderForUserPanel(string userName, int orderId)
        {
            int userId = _userService.GetUserIdByUserName(userName);

            Order order = _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Course)
                .FirstOrDefault(o => o.UserId == userId && o.OrderId == orderId);

            return order;
        }

        public void UpdateDiscount(Discount discount)
        {
            _context.Discounts.Update(discount);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            _context.Update(order);
            _context.SaveChanges();
        }

        public DiscountEnumReturn UseDiscount(int orderId, string code)
        {
            if(!_context.Discounts.Any(d=>d.DiscountCode == code))
            {
                return DiscountEnumReturn.NotFound;
            }

            var discount = _context.Discounts.SingleOrDefault(d=>d.DiscountCode==code);

            if(discount.StartDate != null && discount.StartDate > DateTime.Now)
            {
                return DiscountEnumReturn.NotBegin;
            }

            if(discount.EndDate != null && discount.EndDate < DateTime.Now)
            {
                return DiscountEnumReturn.ExpierDate;
            }

            if(discount.DiscountCode != null && discount.UsableCount < 1)
            {
                return DiscountEnumReturn.Finished;
            }

            Order order = GetOrderById(orderId);

            if (_context.UserDiscountCodes.Any(u => u.UserId ==order.UserId && u.Discount.DiscountCode == discount.DiscountCode))
            {
                return DiscountEnumReturn.UserUsed;
            }

            order.OrderSum = order.OrderSum - ((order.OrderSum * discount.DiscountPercent) / 100);
            UpdateOrder(order);

            discount.UsableCount -= 1;
            UpdateDiscount(discount);

            _userService.AddUserDiscountCode(order.UserId, discount.DiscountId);

            return DiscountEnumReturn.Success;
        }
    }
}
