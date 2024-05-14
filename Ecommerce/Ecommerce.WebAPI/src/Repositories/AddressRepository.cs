using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.WebAPI.src.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repositories
{
    public class AddressRepository : IBaseRepository<Address, QueryOptions>
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Address> _addresses;

        public AddressRepository(AppDbContext context)
        {
            _context = context;
            _addresses = _context.Addresses;
        }
        public async Task<Address> CreateAsync(Address entity)
        {
            _addresses.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var address = await _addresses.FindAsync(id);
            if (address == null) return false;
            _addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Address>> GetAllAsync(QueryOptions options)
        {
            return await _addresses.ToListAsync();
        }

        public async Task<Address> GetByIdAsync(Guid id)
        {
            return await _addresses.FindAsync(id) ?? throw AppException.NotFound();
        }

        public async Task<Address?> UpdateAsync(Address entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}