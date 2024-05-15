using Ecommerce.Core.src.Entities;
using Ecommerce.WebAPI.src.ExternalService;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Ecommerce.Tests.src.xUnitTests.WebAPI
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _passwordService;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
        private readonly User _user = new User();

        public PasswordServiceTests()
        {
            _passwordService = new PasswordService(_mockPasswordHasher.Object);
        }

        [Fact]
        public void HashPassword_ReturnsNonNullHash()
        {
            // Arrange
            var password = "securePassword123";
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(_user, password))
                .Returns("hashedPassword");

            // Act
            var result = _passwordService.HashPassword(_user, password);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData("hashedPassword", "securePassword123", PasswordVerificationResult.Success)]
        [InlineData("hashedPassword", "wrongPassword", PasswordVerificationResult.Failed)]
        public void VerifyPassword_CorrectlyVerifiesPassword(string hashedPassword, string providedPassword, PasswordVerificationResult expectedResult)
        {
            // Arrange
            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(_user, hashedPassword, providedPassword))
                .Returns(expectedResult);

            // Act
            var result = _passwordService.VerifyPassword(_user, hashedPassword, providedPassword);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }

}