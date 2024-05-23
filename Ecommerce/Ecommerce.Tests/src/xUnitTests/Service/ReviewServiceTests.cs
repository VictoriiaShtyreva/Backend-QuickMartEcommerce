using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Ecommerce.Tests.src.xUnitTests.Service
{
    public class ReviewServiceTests
    {
        private readonly ReviewService _reviewService;
        private readonly Mock<IReviewRepository> _mockReviewRepository = new Mock<IReviewRepository>();
        private readonly Mock<IProductRepository> _mockProductRepository = new Mock<IProductRepository>();
        private readonly Mock<IUserRepository> _mockUserRepository = new Mock<IUserRepository>();
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
        private readonly Mock<IMemoryCache> _mockCache = new Mock<IMemoryCache>();

        public ReviewServiceTests()
        {
            _reviewService = new ReviewService(_mockReviewRepository.Object, _mockMapper.Object, _mockProductRepository.Object, _mockUserRepository.Object, _mockCache.Object);
        }

        [Fact]
        public async Task GetReviewsByProductIdAsync_WithValidProductId_ReturnsReviews()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var reviews = new List<Review> { new Review() };
            _mockReviewRepository.Setup(repo => repo.GetReviewsByProductIdAsync(productId)).ReturnsAsync(reviews);
            _mockMapper.Setup(m => m.Map<IEnumerable<ReviewReadDto>>(reviews)).Returns(new List<ReviewReadDto>());

            // Act
            var result = await _reviewService.GetReviewsByProductIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            _mockReviewRepository.Verify(r => r.GetReviewsByProductIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetReviewsByUserIdAsync_WithValidUserId_ReturnsReviews()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var reviews = new List<Review> { new Review() };
            _mockReviewRepository.Setup(repo => repo.GetReviewsByUserIdAsync(userId)).ReturnsAsync(reviews);
            _mockMapper.Setup(m => m.Map<IEnumerable<ReviewReadDto>>(reviews)).Returns(new List<ReviewReadDto>());

            // Act
            var result = await _reviewService.GetReviewsByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            _mockReviewRepository.Verify(r => r.GetReviewsByUserIdAsync(userId), Times.Once);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public async Task CreateOneAsync_Validations_ReturnsReviewDtoOrThrows(bool userExists, bool productExists)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createDto = new ReviewCreateDto { ProductId = Guid.NewGuid() };
            var review = new Review();
            _mockUserRepository.Setup(repo => repo.ExistsAsync(It.IsAny<User>())).ReturnsAsync(userExists);
            _mockProductRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Product>())).ReturnsAsync(productExists);
            _mockReviewRepository.Setup(repo => repo.CreateAsync(It.IsAny<Review>())).ReturnsAsync(review);
            _mockMapper.Setup(m => m.Map<Review>(createDto)).Returns(review);
            _mockMapper.Setup(m => m.Map<ReviewReadDto>(review)).Returns(new ReviewReadDto());

            if (!userExists || !productExists)
            {
                // Act & Assert
                await Assert.ThrowsAsync<AppException>(() => _reviewService.CreateReviewAsync(userId, createDto));
            }
            else
            {
                // Act
                var result = await _reviewService.CreateReviewAsync(userId, createDto);

                // Assert
                Assert.NotNull(result);
                _mockReviewRepository.Verify(r => r.CreateAsync(It.IsAny<Review>()), Times.Once);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DeleteOneAsync_ReturnsTrueOrThrowsNotFound(bool exists)
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockReviewRepository.Setup(r => r.DeleteAsync(id)).ReturnsAsync(exists);

            if (!exists)
            {
                // Act & Assert
                var result = await _reviewService.DeleteOneAsync(id);
                Assert.False(result);
            }
            else
            {
                // Act
                var result = await _reviewService.DeleteOneAsync(id);

                // Assert
                Assert.True(result);
                _mockReviewRepository.Verify(r => r.DeleteAsync(id), Times.Once);
            }
        }

        [Fact]
        public async Task UpdateOneAsync_UpdatesReview_WhenExists()
        {
            var reviewId = Guid.NewGuid();
            var review = new Review { Id = reviewId };
            var updateDto = new ReviewUpdateDto();

            _mockReviewRepository.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);
            _mockReviewRepository.Setup(r => r.UpdateAsync(review)).ReturnsAsync(review);
            _mockMapper.Setup(m => m.Map(updateDto, review)).Returns(review);
            _mockMapper.Setup(m => m.Map<ReviewReadDto>(review)).Returns(new ReviewReadDto());

            var result = await _reviewService.UpdateOneAsync(reviewId, updateDto);

            Assert.NotNull(result);
            _mockReviewRepository.Verify(r => r.GetByIdAsync(reviewId), Times.Once);
            _mockReviewRepository.Verify(r => r.UpdateAsync(review), Times.Once);
        }
    }
}
