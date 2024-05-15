using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.src.Common
{
    public class UserCredential
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        public UserCredential(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}