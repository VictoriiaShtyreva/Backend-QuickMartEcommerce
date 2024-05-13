using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Entities.CartAggregate;
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
        private readonly DbSet<Cart> _carts;
        public UserRepository(AppDbContext context)
        {
            _context = context;
            _users = _context.Users;
            _carts = _context.Carts;
        }

        public async Task<User> CreateAsync(User entity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            _users.Add(entity);
            await _context.SaveChangesAsync();
            var cart = new Cart { UserId = entity.Id };
            _carts.Add(cart);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            var user = await _context.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                await transaction.RollbackAsync();
                return false;
            }
            // Delete the cart if it exists
            if (user.Cart != null)
            {
                _context.Carts.Remove(user.Cart);
            }
            // Delete the user
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
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