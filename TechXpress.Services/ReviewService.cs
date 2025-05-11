using TechXpress.Data.Entities;
using TechXpress.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TechXpress.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Review>> GetReviewsForProductAsync(int productId)
        {
            if (productId == 0)
                return (await _unitOfWork.Reviews
                    .GetAllAsync(include: q => q.Include(r => r.User).Include(r => r.Product)))
                    .OrderByDescending(r => r.Date);
            return (await _unitOfWork.Reviews
                .FindAsync(r => r.ProductId == productId, include: q => q.Include(r => r.User).Include(r => r.Product)))
                .OrderByDescending(r => r.Date);
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
            => await _unitOfWork.Reviews.GetByIdAsync(id);

        public async Task AddReviewAsync(Review review)
        {
            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateReviewAsync(Review review)
        {
            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review != null)
            {
                _unitOfWork.Reviews.Remove(review);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> CanUserReviewProductAsync(string userId, int productId)
        {
            // User can review if they have an order with this product
            var orders = await _unitOfWork.Orders.FindAsync(o => o.UserId == userId && o.OrderDetails.Any(od => od.ProductId == productId));
            return orders.Any();
        }
    }
} 