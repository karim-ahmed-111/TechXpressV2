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
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Category>> _categoryRepoMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _categoryRepoMock = new Mock<IRepository<Category>>();
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _service = new CategoryService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsAllCategories()
        {
            var categories = new List<Category> { new Category { Id = 1 }, new Category { Id = 2 } };
            _categoryRepoMock.Setup(r => r.GetAllAsync(null)).ReturnsAsync(categories);
            var result = await _service.GetAllCategoriesAsync();
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnsCategory()
        {
            var category = new Category { Id = 1 };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);
            var result = await _service.GetCategoryByIdAsync(1);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task AddCategoryAsync_AddsCategory()
        {
            var category = new Category { Id = 1 };
            _categoryRepoMock.Setup(r => r.AddAsync(category)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            await _service.AddCategoryAsync(category);
            _categoryRepoMock.Verify(r => r.AddAsync(category), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_UpdatesCategory()
        {
            var category = new Category { Id = 1, Name = "Old" };
            _categoryRepoMock.Setup(r => r.Update(category));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            await _service.UpdateCategoryAsync(category);
            _categoryRepoMock.Verify(r => r.Update(category), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_DeletesCategory()
        {
            var category = new Category { Id = 1 };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);
            _categoryRepoMock.Setup(r => r.Remove(category));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            await _service.DeleteCategoryAsync(1);
            _categoryRepoMock.Verify(r => r.Remove(category), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
} 