using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.WebAPI.src.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repositories
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
            _users.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _users.FindAsync(id);
            if (user == null) return false;

            // Find and delete related reviews
            var relatedReviews = _context.Reviews.Where(r => r.UserId == id);
            _context.Reviews.RemoveRange(relatedReviews);

            // Find and delete related orders
            var relatedOrders = _context.Orders.Where(o => o.UserId == id);
            _context.Orders.RemoveRange(relatedOrders);

            // Remove the user
            _users.Remove(user);

            // Save changes
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<User>> GetAllAsync(QueryOptions options)
        {
            IQueryable<User> query = _users;

            // Get total count
            var totalCount = await query.CountAsync();

            // Sorting
            query = SortUsers(query, options.SortBy, options.SortOrder);

            // Pagination
            query = query.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);

            var users = await query.ToListAsync();

            return new PaginatedResult<User>(users, totalCount);
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
            return await _users
                .Include(u => u.Orders)
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == id) ?? throw AppException.NotFound();
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

        public async Task<User?> UpdateAsync(User entity)
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

        public async Task<bool> ExistsAsync(User entity)
        {
            return await _users.AnyAsync(e => e.Id == entity.Id);
        }
    }
}