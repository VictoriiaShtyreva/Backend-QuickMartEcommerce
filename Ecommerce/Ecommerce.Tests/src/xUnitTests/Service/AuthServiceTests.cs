using Moq;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.Services;
using Ecommerce.Service.src.Interfaces;
using Ecommerce.Core.src.Entities;
using Microsoft.AspNetCore.Identity;
using Ecommerce.Core.src.Common;
using AutoMapper;
using Ecommerce.Service.src.DTOs;

namespace Ecommerce.Tests.src.xUnitTests.Service
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;
        private readonly Mock<IUserRepository> _mockUserRepository = new Mock<IUserRepository>();
        private readonly Mock<ITokenService> _mockTokenService = new Mock<ITokenService>();
        private readonly Mock<IPasswordService> _mockPasswordService = new Mock<IPasswordService>();
        private readonly IMapper _mapper;

        public AuthServiceTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Ecommerce.Service.src.Shared.AutoMapperProfile());
            });
            _mapper = config.CreateMapper();

            _authService = new AuthService(_mockUserRepository.Object, _mockTokenService.Object, _mockPasswordService.Object, _mapper);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ReturnsUserReadDto_WhenTokenIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "test@example.com" };
            var userReadDto = new UserReadDto { Id = userId, Email = "test@example.com" };

            _mockTokenService.Setup(x => x.VerifyToken(It.IsAny<string>())).Returns(userId);
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateUserAsync("valid-token");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userReadDto.Id, result.Id);
            Assert.Equal(userReadDto.Email, result.Email);
        }

        public static IEnumerable<object[]> LogInAsyncData =>
            new List<object[]>
            {
                new object[] { "user@example.com", "correctpassword", PasswordVerificationResult.Success, true, null! },
                new object[] { "user@example.com", "wrongpassword", PasswordVerificationResult.Failed, false, typeof(AppException) },
                new object[] { "notfound@example.com", "any", PasswordVerificationResult.Failed, false, typeof(AppException) }
            };

        [Theory]
        [MemberData(nameof(LogInAsyncData))]
        public async Task LogInAsync_HandlesScenarios_Correctly(string email, string password, PasswordVerificationResult result, bool shouldSucceed, Type exceptionType)
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com", Password = "hashedpassword" };
            var userCredential = new UserCredential(email, password);

            if (shouldSucceed || (exceptionType == typeof(AppException) && result == PasswordVerificationResult.Failed))
            {
                _mockUserRepository.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
                _mockPasswordService.Setup(x => x.VerifyPassword(user, user.Password, password)).Returns(result);
            }
            else
            {
                _mockUserRepository.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync((User)null!);
            }

            if (shouldSucceed)
            {
                _mockTokenService.Setup(x => x.GetToken(It.IsAny<User>())).Returns("valid-token");
            }

            // Act & Assert
            if (shouldSucceed)
            {
                var token = await _authService.LogInAsync(userCredential);
                Assert.Equal("valid-token", token);
            }
            else if (exceptionType != null)
            {
                await Assert.ThrowsAsync(exceptionType, () => _authService.LogInAsync(userCredential));
            }
        }

        [Fact]
        public async Task LogoutAsync_ReturnsInvalidateTokenResult()
        {
            // Arrange
            _mockTokenService.Setup(x => x.InvalidateTokenAsync()).ReturnsAsync("token-invalidated");

            // Act
            var result = await _authService.LogoutAsync();

            // Assert
            Assert.Equal("token-invalidated", result);
        }
    }
}