using TechXpress.Data.Entities;
using TechXpress.Data.Repositories;

namespace TechXpress.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync() => await _unitOfWork.Products.GetAllAsync();

        public async Task<Product?> GetProductByIdAsync(int id) => await _unitOfWork.Products.GetByIdAsync(id);

        public async Task AddProductAsync(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existing = await _unitOfWork.Products.GetByIdAsync(product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.Stock = product.Stock;
                existing.CategoryId = product.CategoryId;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                _unitOfWork.Products.Remove(product);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
} 