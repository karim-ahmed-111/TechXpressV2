using TechXpress.Data.Entities;
using TechXpress.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TechXpress.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Order> PlaceOrderAsync(string userId, string shippingAddress, string stripePaymentId)
        {
            var cart = (await _unitOfWork.Carts.FindAsync(c => c.UserId == userId)).FirstOrDefault();
            if (cart == null || !cart.CartItems.Any()) throw new Exception("Cart is empty");
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Paid",
                TotalAmount = cart.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity),
                StripePaymentId = stripePaymentId,
                ShippingAddress = shippingAddress,
                OrderDetails = cart.CartItems.Select(ci => new OrderDetail
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                }).ToList()
            };
            await _unitOfWork.Orders.AddAsync(order);
            // Decrement inventory
            foreach (var item in cart.CartItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    _unitOfWork.Products.Update(product);
                }
            }
            // Clear cart
            foreach (var item in cart.CartItems.ToList())
            {
                _unitOfWork.CartItems.Remove(item);
            }
            await _unitOfWork.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(string userId)
            => (await _unitOfWork.Orders.FindAsync(o => o.UserId == userId)).OrderByDescending(o => o.OrderDate);

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
            => await _unitOfWork.Orders.GetAllAsync(include: q => q.Include(o => o.User));

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            var orders = await _unitOfWork.Orders.FindAsync(
                o => o.Id == id,
                include: q => q.Include(o => o.User)
                          .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
            );
            return orders.FirstOrDefault();
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order != null && order.UserId == userId && order.Status == "Pending")
            {
                order.Status = "Cancelled";
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
} 