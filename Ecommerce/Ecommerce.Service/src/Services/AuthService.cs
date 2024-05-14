using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Service.src.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IPasswordService passwordService)
        {
            _userRepo = userRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }
        public async Task<User> AuthenticateUserAsync(string token)
        {
            var userId = _tokenService.VerifyToken(token);
            var user = await _userRepo.GetByIdAsync(userId);
            return user;
        }

        public async Task<string> LogInAsync(UserCredential userCredential)
        {
            var foundByEmail = await _userRepo.GetByEmailAsync(userCredential.Email) ?? throw AppException.NotFound();
            var isPasswordMatch = _passwordService.VerifyPassword(foundByEmail, foundByEmail.Password!, userCredential.Password);
            if (isPasswordMatch == PasswordVerificationResult.Failed)
            {
                throw AppException.InvalidLoginCredentialsException();
            }
            return _tokenService.GetToken(foundByEmail);
        }

        public async Task<string> LogoutAsync()
        {
            return await _tokenService.InvalidateTokenAsync();
        }
    }
}