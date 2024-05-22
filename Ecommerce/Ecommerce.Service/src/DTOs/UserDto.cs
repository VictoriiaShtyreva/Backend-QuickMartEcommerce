using Ecommerce.Core.src.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Service.src.DTOs
{

    public class UserReadDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public UserRole Role { get; set; }
        public string? Avatar { get; set; }
        public IEnumerable<OrderReadDto>? Orders { get; set; }
        public IEnumerable<ReviewReadDto>? Reviews { get; set; }
    }

    public class UserUpdateDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public UserRole Role { get; set; }
        public IFormFile? Avatar { get; set; }
    }

    public class UserCreateDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public IFormFile? Avatar { get; set; }
        public UserRole Role { get; set; }
    }

    public class UserRoleUpdateDto
    {
        public UserRole NewRole { get; set; }
    }

}