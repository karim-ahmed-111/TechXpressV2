using TechXpress.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace TechXpress.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TechXpressDbContext _context;
        public IRepository<Product> Products { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<Order> Orders { get; }
        public IRepository<OrderDetail> OrderDetails { get; }
        public IRepository<Review> Reviews { get; }
        public IRepository<Cart> Carts { get; }
        public IRepository<CartItem> CartItems { get; }

        public UnitOfWork(TechXpressDbContext context)
        {
            _context = context;
            Products = new Repository<Product>(_context);
            Categories = new Repository<Category>(_context);
            Orders = new Repository<Order>(_context);
            OrderDetails = new Repository<OrderDetail>(_context);
            Reviews = new Repository<Review>(_context);
            Carts = new Repository<Cart>(_context);
            CartItems = new Repository<CartItem>(_context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();

        public async Task<Cart?> GetCartWithItemsAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
} 