using Ecommerce.Core.src.Entities;

namespace Ecommerce.Service.src.Interfaces
{
    public interface ITokenService
    {
        public string GetToken(User user);
        public Guid VerifyToken(string token);
        Task<string> InvalidateTokenAsync();
    }
}