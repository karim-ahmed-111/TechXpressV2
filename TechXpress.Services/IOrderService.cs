using TechXpress.Data.Entities;

namespace TechXpress.Services
{
    public interface IOrderService
    {
        Task<Order> PlaceOrderAsync(string userId, string shippingAddress, string stripePaymentId);
        Task<IEnumerable<Order>> GetOrdersForUserAsync(string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> CancelOrderAsync(int orderId, string userId);
    }
} 