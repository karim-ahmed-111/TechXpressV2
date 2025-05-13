using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TechXpress.Data.Entities;
using TechXpress.Data.Repositories;
using TechXpress.Services;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace TechXpress.Tests
{
    public class ReviewServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Review>> _reviewRepoMock;
        private readonly ReviewService _service;

        public ReviewServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _reviewRepoMock = new Mock<IRepository<Review>>();
            _unitOfWorkMock.Setup(u => u.Reviews).Returns(_reviewRepoMock.Object);
            _service = new ReviewService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetReviewsForProductAsync_ReturnsReviews()
        {
            var reviews = new List<Review> { new Review { Id = 1, ProductId = 1 }, new Review { Id = 2, ProductId = 1 } };
            _reviewRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Review, bool>>>(), It.IsAny<System.Func<IQueryable<Review>, IQueryable<Review>>>()))
                .ReturnsAsync(reviews);
            var result = await _service.GetReviewsForProductAsync(1);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddReviewAsync_AddsReview()
        {
            var review = new Review { Id = 1 };
            _reviewRepoMock.Setup(r => r.AddAsync(review)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            await _service.AddReviewAsync(review);
            _reviewRepoMock.Verify(r => r.AddAsync(review), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateReviewAsync_UpdatesReview()
        {
            var review = new Review { Id = 1 };
            _reviewRepoMock.Setup(r => r.Update(review));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            await _service.UpdateReviewAsync(review);
            _reviewRepoMock.Verify(r => r.Update(review), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_DeletesReview()
        {
            var review = new Review { Id = 1 };
            _reviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);
            _reviewRepoMock.Setup(r => r.Remove(review));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            await _service.DeleteReviewAsync(1);
            _reviewRepoMock.Verify(r => r.Remove(review), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
} 