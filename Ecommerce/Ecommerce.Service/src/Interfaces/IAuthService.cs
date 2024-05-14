using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;

namespace Ecommerce.Service.src.Interfaces
{
    public interface IAuthService
    {
        Task<string> LogInAsync(UserCredential userCredential);
        Task<string> LogoutAsync();
        Task<User> AuthenticateUserAsync(string token);
    }
}