using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Service.src.Services
{
    public class UserService : BaseService<User, UserReadDto, UserCreateDto, UserUpdateDto, QueryOptions>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;
        public UserService(IUserRepository userRepository, IMapper mapper, IPasswordHasher<User> passwordHasher, IMemoryCache cache)
            : base(userRepository, mapper, cache)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

        }

        // override CreateAsync to hash password
        public override async Task<UserReadDto> CreateOneAsync(UserCreateDto createDto)
        {
            var user = _mapper.Map<User>(createDto);
            user.Password = _passwordHasher.HashPassword(user, createDto.Password);
            user = await _userRepository.CreateAsync(user);
            _cache.Remove($"GetAll-{typeof(User).Name}");
            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<bool> ResetPasswordAsync(Guid userId, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;
            user.Password = _passwordHasher.HashPassword(user, newPassword);
            await _userRepository.UpdateAsync(user);
            _cache.Remove(user.Email!);
            return true;
        }

        public async Task<bool> UpdatePasswordAsync(Guid userId, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw AppException.NotFound();
            user.Password = _passwordHasher.HashPassword(user, newPassword);
            await _userRepository.UpdateAsync(user);
            _cache.Remove(user.Email!);
            return true;
        }

        public async Task<UserReadDto> UpdateRoleAsync(Guid userId, UserRoleUpdateDto roleUpdateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw AppException.NotFound();
            user.Role = roleUpdateDto.NewRole;
            user = await _userRepository.UpdateAsync(user);
            _cache.Remove(user?.Email!);
            return _mapper.Map<UserReadDto>(user);
        }
    }
}