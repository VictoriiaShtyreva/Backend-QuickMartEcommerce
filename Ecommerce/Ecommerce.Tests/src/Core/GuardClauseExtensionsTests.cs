using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using Ecommerce.Core.src.Extensions;
using FluentAssertions;

namespace Ecommerce.Tests.src.Core
{
    public class GuardClauseExtensionsTests
    {
        [Theory]
        [InlineData("example@example.com", true)]
        [InlineData("example.com", false)]
        [InlineData(" ", false)]
        [InlineData(null, false)]
        public void TestInvalidEmail(string input, bool isValid)
        {
            if (isValid)
            {
                var result = Guard.Against.InvalidEmail(input, nameof(input));
                Assert.Equal(input, result);
            }
            else
            {
                Assert.Throws<ArgumentException>(() => Guard.Against.InvalidEmail(input, nameof(input)));
            }
        }

        [Theory]
        [InlineData("example@example.com", true)]
        [InlineData("user.name@domain.com", true)]
        [InlineData("user_name@domain.co.in", true)]
        [InlineData("name@domain.com", true)]
        [InlineData("name@domain", false)]
        [InlineData("justastring", false)]
        [InlineData("@missingusername.com", false)]
        [InlineData("missingatsign.com", false)]
        [InlineData("missingdomain@", false)]
        [InlineData("user name@domain.com", false)]
        [InlineData("user@domain com", false)]
        [InlineData("user@domain.com something", false)]
        public void EmailRegex_Should_Correctly_Validate_Emails(string email, bool isValid)
        {
            // Arrange
            Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            // Act
            bool result = emailRegex.IsMatch(email);

            // Assert
            result.Should().Be(isValid);
        }
    }
}