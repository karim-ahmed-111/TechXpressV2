using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TechXpress.Data.Entities;
using TechXpress.Data.Repositories;
using TechXpress.Services;
using Xunit;

namespace TechXpress.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Product>> _productRepoMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productRepoMock = new Mock<IRepository<Product>>();
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _service = new ProductService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProducts()
        {
            var products = new List<Product> { new Product { Id = 1 }, new Product { Id = 2 } };
            _productRepoMock.Setup(r => r.GetAllAsync(null)).ReturnsAsync(products);
            var result = await _service.GetAllProductsAsync();
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsProduct()
        {
            var product = new Product { Id = 1 };
            _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            var result = await _service.GetProductByIdAsync(1);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task AddProductAsync_AddsProduct()
        {
            var product = new Product { Id = 1 };
            _productRepoMock.Setup(r => r.AddAsync(product)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            await _service.AddProductAsync(product);
            _productRepoMock.Verify(r => r.AddAsync(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdatesProduct()
        {
            var product = new Product { Id = 1, Name = "Old" };
            _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            var updated = new Product { Id = 1, Name = "New" };
            await _service.UpdateProductAsync(updated);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal("New", product.Name);
        }

        [Fact]
        public async Task DeleteProductAsync_DeletesProduct()
        {
            var product = new Product { Id = 1 };
            _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            await _service.DeleteProductAsync(1);
            _productRepoMock.Verify(r => r.Remove(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
} 