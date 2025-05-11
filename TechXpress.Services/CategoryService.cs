using TechXpress.Data.Entities;
using TechXpress.Data.Repositories;

namespace TechXpress.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync() => await _unitOfWork.Categories.GetAllAsync();

        public async Task<Category?> GetCategoryByIdAsync(int id) => await _unitOfWork.Categories.GetByIdAsync(id);

        public async Task AddCategoryAsync(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category != null)
            {
                _unitOfWork.Categories.Remove(category);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
} 