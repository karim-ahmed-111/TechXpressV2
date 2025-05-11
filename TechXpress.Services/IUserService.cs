using TechXpress.Data.Entities;

namespace TechXpress.Services
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task UpdateUserProfileAsync(ApplicationUser user);
    }
} 