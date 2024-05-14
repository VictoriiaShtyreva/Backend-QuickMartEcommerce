using Ecommerce.Core.src.Entities;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Service.src.Interfaces
{
    public interface IPasswordService
    {
        string HashPassword(User user, string password);
        PasswordVerificationResult VerifyPassword(User user, string hashedPassword, string providedPassword);
    }
}