using TechXpress.Data.Entities;

namespace TechXpress.Services
{
    public interface ICartService
    {
        Task<Cart> GetOrCreateCartAsync(string userId);
        Task AddToCartAsync(string userId, int productId, int quantity);
        Task RemoveFromCartAsync(string userId, int productId);
        Task UpdateCartItemAsync(string userId, int productId, int quantity);
        Task ClearCartAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
        Task UpdateCartItemQuantityAsync(string userId, int cartItemId, int quantity);
    }
} 