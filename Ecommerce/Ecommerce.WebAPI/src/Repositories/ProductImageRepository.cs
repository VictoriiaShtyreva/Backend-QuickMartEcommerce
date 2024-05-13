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
            if (productImage == null || productImage.ProductId != Guid.Empty) return false;
            _productImages.Remove(productImage);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductImage>> GetAllAsync(QueryOptions options)
        {
            return await _productImages.ToListAsync();
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