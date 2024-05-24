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
        private readonly ICloudinaryImageService _imageService;
        private readonly MemoryCacheEntryOptions _cacheOptions;
        public UserService(IUserRepository userRepository, IMapper mapper, IPasswordHasher<User> passwordHasher, IMemoryCache cache, ICloudinaryImageService imageService)
            : base(userRepository, mapper, cache)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            _imageService = imageService;

        }

        // override CreateAsync to hash password
        public override async Task<UserReadDto> CreateOneAsync(UserCreateDto createDto)
        {
            var user = _mapper.Map<User>(createDto);
            if (createDto.Avatar != null)
            {
                var uploadResult = await _imageService.UploadImageAsync(createDto.Avatar);
                user.Avatar = uploadResult.SecureUrl.ToString();
            }
            user.Password = _passwordHasher.HashPassword(user, createDto.Password);
            user = await _userRepository.CreateAsync(user);
            _cache.Remove($"GetAll-{typeof(User).Name}");
            return _mapper.Map<UserReadDto>(user);
        }

        public override async Task<UserReadDto> UpdateOneAsync(Guid id, UserUpdateDto updateDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id) ?? throw AppException.NotFound();
            if (!string.IsNullOrEmpty(updateDto.Email))
            {
                existingUser.Email = updateDto.Email;
            }
            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                existingUser.Name = updateDto.Name;
            }
            if (updateDto.Avatar != null)
            {
                if (!string.IsNullOrEmpty(existingUser.Avatar))
                {
                    // Extract public ID from the existing avatar URL to delete it
                    var publicId = new Uri(existingUser.Avatar).Segments.Last().Split('.').First();
                    await _imageService.DeleteImageAsync(publicId);
                }
                var uploadResult = await _imageService.UploadImageAsync(updateDto.Avatar);
                existingUser.Avatar = uploadResult.SecureUrl.ToString();
            }
            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            return _mapper.Map<UserReadDto>(updatedUser);
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