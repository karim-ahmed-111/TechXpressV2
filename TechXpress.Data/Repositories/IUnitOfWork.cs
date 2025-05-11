using TechXpress.Data.Entities;

namespace TechXpress.Data.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }
        IRepository<Category> Categories { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderDetail> OrderDetails { get; }
        IRepository<Review> Reviews { get; }
        IRepository<Cart> Carts { get; }
        IRepository<CartItem> CartItems { get; }
        Task<int> SaveChangesAsync();
        Task<Cart?> GetCartWithItemsAsync(string userId);
    }
} 