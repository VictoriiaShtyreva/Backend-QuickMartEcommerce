using System.Net;
using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Ecommerce.Tests.src.Service
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<IUserRepository> _mockUserRepository = new Mock<IUserRepository>();
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
        private readonly Mock<IMemoryCache> _mockCache = new Mock<IMemoryCache>();
        private readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
        public UserServiceTests()
        {
            _userService = new UserService(_mockUserRepository.Object, _mockMapper.Object, _mockPasswordHasher.Object, _mockCache.Object);
        }

        private User CreateTestUser()
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Password = "oldPasswordHash"
            };
        }

        private void SetupCache<T>(string key, T value)
        {
            var cacheEntry = Mock.Of<ICacheEntry>();
            _mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntry);
            _mockCache.Setup(m => m.TryGetValue(key, out value!)).Returns(true);
        }

        [Fact]
        public async Task CreateOneAsync_ShouldCreateUser_WhenValidInput()
        {
            // Arrange
            var createUserDto = new UserCreateDto { Password = "password123", Name = "John Doe", Email = "john@example.com" };
            var createdUser = new User { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com", Password = "hashedPassword" };

            _mockMapper.Setup(m => m.Map<User>(It.IsAny<UserCreateDto>())).Returns(createdUser);
            _mockPasswordHasher.Setup(h => h.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashedPassword");
            _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(createdUser);
            _mockMapper.Setup(m => m.Map<UserReadDto>(It.IsAny<User>())).Returns(new UserReadDto { Name = "John Doe", Email = "john@example.com" });

            // Act
            var result = await _userService.CreateOneAsync(createUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.Name);
            _mockUserRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
            _mockCache.Verify(c => c.Remove(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateRoleAsync_UpdatesUserRole()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var originalUser = new User { Id = userId, Name = "Original Name", Email = "original@example.com", Role = UserRole.Customer };
            var updatedRoleDto = new UserRoleUpdateDto { NewRole = UserRole.Admin };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(originalUser);
            _mockUserRepository.Setup(repo => repo.UpdateAsync(It.Is<User>(u => u.Role == UserRole.Admin))).ReturnsAsync((User u) => u);
            _mockMapper.Setup(mapper => mapper.Map<UserReadDto>(It.Is<User>(u => u.Role == UserRole.Admin)))
                       .Returns((User u) => new UserReadDto { Role = u.Role });

            // Act
            var result = await _userService.UpdateRoleAsync(userId, updatedRoleDto);

            // Assert
            Assert.Equal(UserRole.Admin, result.Role);
            _mockUserRepository.Verify(repo => repo.UpdateAsync(It.Is<User>(u => u.Role == UserRole.Admin)), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<UserReadDto>(It.Is<User>(u => u.Role == UserRole.Admin)), Times.Once);
            _mockCache.Verify(c => c.Remove(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOneAsync_UpdatesOnlyProvidedFields_WhenCalledWithPartialData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User { Id = userId, Name = "Original Name", Email = "original@example.com" };
            var updateDto = new UserUpdateDto { Name = "Updated Name" }; // Only update name
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            _mockMapper.Setup(m => m.Map(It.IsAny<UserUpdateDto>(), It.IsAny<User>()))
                       .Callback<UserUpdateDto, User>((dto, user) => user.Name = dto.Name ?? user.Name);
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(existingUser);
            _mockMapper.Setup(m => m.Map<UserReadDto>(It.IsAny<User>())).Returns(new UserReadDto { Name = "Updated Name" });

            // Act
            var result = await _userService.UpdateOneAsync(userId, updateDto);

            // Assert
            Assert.Equal("Updated Name", result.Name);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
            _mockCache.Verify(c => c.Remove(It.IsAny<string>()), Times.Exactly(3));
        }

        [Fact]
        public async Task UpdateOneAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var updateDto = new UserUpdateDto { Name = "Updated Name" };
            Guid userId = Guid.NewGuid();

            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null!);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _userService.UpdateOneAsync(userId, updateDto));
        }

        [Fact]
        public async Task UpdatePasswordAsync_ShouldUpdatePassword_WhenUserExists()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string newPassword = "newSecurePassword123";
            var existingUser = new User { Id = userId, Password = "oldPassword" };

            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existingUser);
            _mockPasswordHasher.Setup(h => h.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashedNewPassword");
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(existingUser);

            // Act
            var result = await _userService.UpdatePasswordAsync(userId, newPassword);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
            _mockCache.Verify(c => c.Remove(existingUser.Email!), Times.Once);
        }

        [Theory]
        [InlineData(true, true, null)] // Test case for successful deletion
        [InlineData(false, false, null)] // Test case for failed deletion (user not found)
        [InlineData(false, true, typeof(AppException), HttpStatusCode.NotFound)] // Test case for exception handling
        public async Task DeleteOneAsync_HandlesVariousScenarios_Correctly(bool expectedResult, bool repoSetupResult, Type? exceptionType, HttpStatusCode? httpStatusCode = null)
        {
            // Arrange
            var userId = Guid.NewGuid();
            if (exceptionType == null)
            {
                _mockUserRepository.Setup(r => r.DeleteAsync(userId)).ReturnsAsync(repoSetupResult);
            }
            else
            {
                if (exceptionType == typeof(AppException) && httpStatusCode.HasValue)
                {
                    var exceptionInstance = (Exception)Activator.CreateInstance(exceptionType, httpStatusCode.Value, "Simulated database error")!;
                    _mockUserRepository.Setup(r => r.DeleteAsync(userId)).ThrowsAsync(exceptionInstance);
                }
            }

            // Act & Assert
            if (exceptionType == null)
            {
                var result = await _userService.DeleteOneAsync(userId);
                Assert.Equal(expectedResult, result);
            }
            else
            {
                var exceptionAssertion = await Assert.ThrowsAsync(exceptionType, () => _userService.DeleteOneAsync(userId));
                Assert.IsType<AppException>(exceptionAssertion);
                Assert.Equal(httpStatusCode, ((AppException)exceptionAssertion).StatusCode);
            }

            _mockUserRepository.Verify(r => r.DeleteAsync(userId), Times.Once);
            _mockCache.Verify(c => c.Remove(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var user = CreateTestUser();
            var newPassword = "newPassword123";
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(user, newPassword)).Returns("newHashedPassword");

            // Act
            var result = await _userService.ResetPasswordAsync(user.Id, newPassword);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(user.Id), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(user, newPassword), Times.Once);
            _mockUserRepository.Verify(repo => repo.UpdateAsync(user), Times.Once);
            _mockCache.Verify(c => c.Remove(user.Email!), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var newPassword = "newPassword123";
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null!);

            // Act
            var result = await _userService.ResetPasswordAsync(userId, newPassword);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockUserRepository.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedEntities()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "user1@example.com" },
                new User { Id = Guid.NewGuid(), Email = "user2@example.com" }
            };
            var userDtos = new List<UserReadDto>
            {
                new UserReadDto { Email = "user1@example.com" },
                new UserReadDto { Email = "user2@example.com" }
            };

            _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions>())).ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<IEnumerable<UserReadDto>>(users)).Returns(userDtos);

            // Act
            var result = await _userService.GetAllAsync(new QueryOptions());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDtos.Count, result.Count());
            _mockUserRepository.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions>()), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<UserReadDto>>(users), Times.Once);
        }

        [Fact]
        public async Task GetOneByIdAsync_ReturnsEntity_WhenFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "user@example.com" };
            var userDto = new UserReadDto { Email = "user@example.com" };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserReadDto>(user)).Returns(userDto);

            // Act
            var result = await _userService.GetOneByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _mockMapper.Verify(m => m.Map<UserReadDto>(user), Times.Once);
            _mockCache.Verify(c => c.TryGetValue($"GetById-{userId}", out It.Ref<User>.IsAny!), Times.Once);
            _mockCache.Verify(c => c.Set($"GetById-{userId}", user, It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
        }

        [Fact]
        public async Task GetOneByIdAsync_ThrowsNotFoundException_WhenNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null!);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _userService.GetOneByIdAsync(userId));
        }
    }
}