using TechXpress.Data.Entities;

namespace TechXpress.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetReviewsForProductAsync(int productId);
        Task<Review?> GetReviewByIdAsync(int id);
        Task AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int id);
        Task<bool> CanUserReviewProductAsync(string userId, int productId);
    }
} 