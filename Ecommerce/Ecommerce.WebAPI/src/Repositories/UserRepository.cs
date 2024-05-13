using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.WebAPI.src.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repo
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<User> _users;
        public UserRepository(AppDbContext context)
        {
            _context = context;
            _users = _context.Users;
        }

        public async Task<User> CreateAsync(User entity)
        {
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetAllAsync(QueryOptions options)
        {
            IQueryable<User> query = _users;
            // Sorting
            query = SortUsers(query, options.SortBy, options.SortOrder);
            // Pagination
            query = query.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);
            return await query.ToListAsync();
        }

        private static IQueryable<User> SortUsers(IQueryable<User> query, SortType sortBy, SortOrder sortOrder)
        {
            switch (sortBy)
            {
                case SortType.byName:
                    query = sortOrder == SortOrder.Ascending ?
                        query.OrderBy(u => u.Name) :
                        query.OrderByDescending(u => u.Name);
                    break;
                default:
                    query = sortOrder == SortOrder.Ascending ?
                     query.OrderBy(u => u.Name) :
                     query.OrderByDescending(u => u.Name);
                    break;
            }
            return query;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _users.FirstOrDefaultAsync(u => u.Email == email) ?? throw AppException.NotFound();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _users.FindAsync(id) ?? throw AppException.NotFound(); ;
        }

        public async Task<User> GetUserByCredentialsAsync(UserCredential userCredential)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == userCredential.Email && u.Password == userCredential.Password) ?? throw AppException.InvalidLoginCredentialsException();
        }

        public async Task<bool> ResetPasswordAsync(Guid userId, string newPassword)
        {
            var user = await _users.FindAsync(userId);
            if (user == null)
                return false;
            user.Password = newPassword;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> UpdateAsync(User entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateUserRoleAsync(Guid userId, UserRole newRole)
        {
            var user = await _users.FindAsync(userId);
            if (user == null)
                return false;
            user.Role = newRole;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}