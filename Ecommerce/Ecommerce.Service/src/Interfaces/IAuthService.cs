using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;

namespace Ecommerce.Service.src.Interfaces
{
    public interface IAuthService
    {
        Task<string> LogInAsync(UserCredential userCredential);
        Task<string> LogoutAsync();
        Task<UserReadDto> AuthenticateUserAsync(string token);
    }
}