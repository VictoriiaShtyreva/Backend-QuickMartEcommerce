using Ardalis.GuardClauses;
using Ecommerce.Core.src.Extensions;

namespace Ecommerce.Core.src.Common
{
    public class UserCredential
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public UserCredential(string email, string password)
        {
            Email = Guard.Against.InvalidEmail(email, nameof(Email));
            Password = Guard.Against.NullOrEmpty(password, nameof(Password));
        }
    }
}