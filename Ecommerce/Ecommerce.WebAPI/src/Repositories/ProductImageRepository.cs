using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.WebAPI.src.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<ProductImage> _productImages;

        public ProductImageRepository(AppDbContext context)
        {
            _context = context;
            _productImages = _context.ProductImages;
        }
        public async Task<ProductImage> CreateAsync(ProductImage entity)
        {
            _productImages.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var productImage = await _productImages.FindAsync(id);
            if (productImage == null) return false;
            _productImages.Remove(productImage);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(ProductImage entity)
        {
            return await _productImages.AnyAsync(e => e.Id == entity.Id);
        }

        public async Task<PaginatedResult<ProductImage>> GetAllAsync(QueryOptions options)
        {
            IQueryable<ProductImage> query = _productImages;

            // Get total count
            var totalCount = await query.CountAsync();

            // Pagination
            query = query.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);

            var productImages = await query.ToListAsync();

            return new PaginatedResult<ProductImage>(productImages, totalCount);
        }

        public async Task<ProductImage> GetByIdAsync(Guid id)
        {
            return await _productImages.FindAsync(id) ?? throw AppException.NotFound();
        }

        public async Task<IEnumerable<ProductImage>> GetProductImagesByProductIdAsync(Guid productId)
        {
            return await _productImages
               .Where(image => image.ProductId == productId)
               .ToListAsync();
        }

        public async Task<ProductImage?> UpdateAsync(ProductImage entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}