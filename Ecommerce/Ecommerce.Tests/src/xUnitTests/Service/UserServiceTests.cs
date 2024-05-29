using System.Net;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Ecommerce.Service.src.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
        private readonly Mock<ICloudinaryImageService> _mockCloudinaryImageService = new Mock<ICloudinaryImageService>();
        public UserServiceTests()
        {
            _userService = new UserService(_mockUserRepository.Object, _mockMapper.Object, _mockPasswordHasher.Object, _mockCache.Object, _mockCloudinaryImageService.Object);
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
        public async Task UpdateOneAsync_ShouldUpdateEmailNameAndAvatar_WhenValidInput()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = CreateTestUser();
            var updateDto = new UserUpdateDto
            {
                Email = "updated@example.com",
                Name = "Updated User",
                Avatar = new FormFile(null!, 0, 0, null!, "avatar.jpg")
            };
            var updatedUser = new User
            {
                Id = userId,
                Email = updateDto.Email,
                Name = updateDto.Name,
                Avatar = "http://example.com/new-avatar.jpg"
            };


            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(existingUser)).ReturnsAsync(updatedUser);

            _mockCloudinaryImageService.Setup(i => i.UploadImageAsync(updateDto.Avatar))
                             .ReturnsAsync(new ImageUploadResult { SecureUrl = new Uri("http://example.com/new-avatar.jpg") });

            _mockMapper.Setup(m => m.Map<UserReadDto>(updatedUser)).Returns(new UserReadDto
            {
                Id = userId,
                Email = updatedUser.Email,
                Name = updatedUser.Name,
                Avatar = updatedUser.Avatar
            });

            // Act
            var result = await _userService.UpdateOneAsync(userId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("updated@example.com", result.Email);
            Assert.Equal("Updated User", result.Name);
            Assert.Equal("http://example.com/new-avatar.jpg", result.Avatar);
            _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _mockUserRepository.Verify(r => r.UpdateAsync(existingUser), Times.Once);
        }

        [Fact]
        public async Task UpdateOneAsync_ShouldThrowNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateDto = new UserUpdateDto { Email = "updated@example.com" };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null!);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _userService.UpdateOneAsync(userId, updateDto));
            _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateOneAsync_ShouldOnlyUpdateEmail_WhenOnlyEmailProvided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = CreateTestUser();
            var updateDto = new UserUpdateDto { Email = "updated@example.com" };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(existingUser)).ReturnsAsync(existingUser);

            _mockMapper.Setup(m => m.Map<UserReadDto>(existingUser)).Returns(new UserReadDto
            {
                Id = userId,
                Email = updateDto.Email,
                Name = existingUser.Name,
                Avatar = existingUser.Avatar
            });

            // Act
            var result = await _userService.UpdateOneAsync(userId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("updated@example.com", result.Email);
            Assert.Equal(existingUser.Name, result.Name);
            Assert.Equal(existingUser.Avatar, result.Avatar);
            _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _mockUserRepository.Verify(r => r.UpdateAsync(existingUser), Times.Once);
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