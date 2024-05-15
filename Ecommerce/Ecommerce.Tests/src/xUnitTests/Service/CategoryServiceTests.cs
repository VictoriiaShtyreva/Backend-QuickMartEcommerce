using System.Net;
using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Services;
using Moq;

namespace Ecommerce.Tests.src.xUnitTests.Service
{
    public class CategoryServiceTests
    {
        private readonly CategoryService _categoryService;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository = new Mock<ICategoryRepository>();
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();

        public CategoryServiceTests()
        {
            _categoryService = new CategoryService(_mockCategoryRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetProductsByCategoryIdAsync_ReturnsProducts_WhenCategoryExists()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Title = "Product 1" },
                new Product { Id = Guid.NewGuid(), Title = "Product 2" }
            };
            var productReadDtos = products.Select(p => new ProductReadDto { Title = p.Title }).ToList();

            _mockCategoryRepository.Setup(r => r.GetProductsByCategoryIdAsync(categoryId)).ReturnsAsync(products);
            _mockMapper.Setup(m => m.Map<IEnumerable<ProductReadDto>>(products)).Returns(productReadDtos);

            // Act
            var result = await _categoryService.GetProductsByCategoryIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, dto => Assert.Contains(dto.Title, products.Select(p => p.Title)));
        }

        [Fact]
        public async Task GetProductsByCategoryIdAsync_ThrowsNotFound_WhenNoProductsExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _mockCategoryRepository.Setup(r => r.GetProductsByCategoryIdAsync(categoryId)).ReturnsAsync(new List<Product>());

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _categoryService.GetProductsByCategoryIdAsync(categoryId));
        }

        [Fact]
        public async Task CreateOneAsync_CreatesCategory_WhenValidDtoProvided()
        {
            // Arrange
            var createDto = new CategoryCreateDto { Name = "New Category", Image = "http://example.com/image.jpg" };
            var expectedCategory = new Category("New Category", "http://example.com/image.jpg");

            _mockMapper.Setup(m => m.Map<Category>(createDto)).Returns(expectedCategory);
            _mockCategoryRepository.Setup(x => x.CreateAsync(It.IsAny<Category>())).ReturnsAsync(expectedCategory);
            _mockMapper.Setup(m => m.Map<CategoryReadDto>(expectedCategory)).Returns(new CategoryReadDto { Name = "New Category", Image = "http://example.com/image.jpg" });

            // Act
            var result = await _categoryService.CreateOneAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Category", result.Name);
            Assert.Equal("http://example.com/image.jpg", result.Image);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCategories_WhenCalled()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category("Category 1", "http://example.com/image1.jpg"),
                new Category("Category 2", "http://example.com/image2.jpg"),
                new Category("Category 3", "http://example.com/image3.jpg")
            };

            var queryOptions = new QueryOptions { Page = 1, PageSize = 3, SortOrder = SortOrder.Descending };

            _mockCategoryRepository.Setup(x => x.GetAllAsync(queryOptions)).ReturnsAsync(categories);
            _mockMapper.Setup(m => m.Map<IEnumerable<CategoryReadDto>>(categories))
                .Returns(categories.Select(c => new CategoryReadDto { Name = c.Name, Image = c.Image }));

            // Act
            var result = await _categoryService.GetAllAsync(queryOptions);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetOneByIdAsync_ReturnsCategory_WhenValidIdProvided()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category("Category", "http://example.com/image.jpg");

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);
            _mockMapper.Setup(m => m.Map<CategoryReadDto>(category))
                .Returns(new CategoryReadDto { Name = category.Name, Image = category.Image });

            // Act
            var result = await _categoryService.GetOneByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Category", result.Name);
            Assert.Equal("http://example.com/image.jpg", result.Image);
        }

        [Fact]
        public async Task UpdateOneAsync_UpdatesCategory_WhenValidDtoProvided()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var updateDto = new CategoryUpdateDto { Name = "Updated Category", Image = "http://example.com/updated_image.jpg" };
            var category = new Category("Category", "http://example.com/image.jpg");

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);
            _mockCategoryRepository.Setup(x => x.UpdateAsync(It.IsAny<Category>())).ReturnsAsync(category);
            _mockMapper.Setup(m => m.Map<CategoryReadDto>(It.IsAny<Category>()))
                .Returns(new CategoryReadDto { Name = "Updated Category", Image = "http://example.com/updated_image.jpg" });

            // Act
            var result = await _categoryService.UpdateOneAsync(categoryId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Category", result.Name);
            Assert.Equal("http://example.com/updated_image.jpg", result.Image);
        }

        [Theory]
        [InlineData(true, true, null, null)] // Test case for successful deletion
        [InlineData(false, false, null, null)] // Test case for failed deletion (category not found)
        [InlineData(false, true, typeof(AppException), HttpStatusCode.NotFound)] // Test case for exception handling
        public async Task DeleteOneAsync_HandlesVariousScenarios_Correctly(bool expectedResult, bool repoSetupResult, Type? exceptionType, HttpStatusCode? httpStatusCode)
        {
            var categoryId = Guid.NewGuid();
            // Arrange
            if (exceptionType == null)
            {
                _mockCategoryRepository.Setup(r => r.DeleteAsync(categoryId)).ReturnsAsync(repoSetupResult);
            }
            else
            {
                var exceptionInstance = (Exception)Activator.CreateInstance(exceptionType, new object[] { httpStatusCode!, "Simulated database error" })!;
                _mockCategoryRepository.Setup(r => r.DeleteAsync(categoryId)).ThrowsAsync(exceptionInstance);
            }

            // Act & Assert
            if (exceptionType == null)
            {
                var result = await _categoryService.DeleteOneAsync(categoryId);
                Assert.Equal(expectedResult, result);
            }
            else
            {
                var exception = await Assert.ThrowsAsync(exceptionType, () => _categoryService.DeleteOneAsync(categoryId));
                Assert.IsType<AppException>(exception);
                var appException = exception as AppException;
                Assert.Equal(httpStatusCode, appException?.StatusCode);
            }

            _mockCategoryRepository.Verify(r => r.DeleteAsync(categoryId), Times.Once);
        }


    }
}