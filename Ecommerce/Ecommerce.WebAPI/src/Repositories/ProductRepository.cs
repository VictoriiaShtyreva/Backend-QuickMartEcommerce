using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.WebAPI.src.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Product> _products;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
            _products = _context.Products;
        }

        public async Task<Product> CreateAsync(Product entity)
        {
            _products.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _products.FindAsync(id);
            if (product == null) return false;
            // Find and delete related reviews
            var relatedReviews = _context.Reviews.Where(r => r.ProductId == id);
            _context.Reviews.RemoveRange(relatedReviews);
            // Remove the product
            _products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<Product>> GetAllAsync(QueryOptions options)
        {
            var query = _products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .AsQueryable();

            // Get total count
            var totalCount = await query.CountAsync();

            // Sorting
            query = ProductsSorting(query, options.SortBy, options.SortOrder);

            // Pagination
            if (options.Page >= 0 && options.PageSize > 0)
            {
                query = query.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);
            }
            var products = await query.ToListAsync();
            return new PaginatedResult<Product>(products, totalCount);

        }

        private IQueryable<Product> ProductsSorting(IQueryable<Product> query, SortType sortBy, SortOrder sortOrder)
        {
            switch (sortBy)
            {
                case SortType.byTitle:
                    query = sortOrder == SortOrder.Ascending
                        ? query.OrderBy(p => p.Title)
                        : query.OrderByDescending(p => p.Title);
                    break;
                case SortType.byPrice:
                    query = sortOrder == SortOrder.Ascending
                        ? query.OrderBy(p => p.Price)
                        : query.OrderByDescending(p => p.Price);
                    break;
                default:
                    query = sortOrder == SortOrder.Ascending
                        ? query.OrderBy(p => p.Title)
                        : query.OrderByDescending(p => p.Title);
                    break;
            }
            return query;
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var product = await _products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id) ?? throw AppException.NotFound();
            return product;
        }

        public async Task<IEnumerable<Product>> GetMostPurchasedProductsAsync(int topNumber)
        {
            // Retrieve the top purchased product IDs based on completed orders
            var topProductIds = await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.Order!.Status == OrderStatus.Completed)
                .GroupBy(oi => oi.ProductSnapshot!.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantityPurchased = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantityPurchased)
                .Take(topNumber)
                .Select(x => x.ProductId)
                .ToListAsync();

            // Fetch detailed product information for the top products
            var products = await _context.Products
                .Where(p => topProductIds.Contains(p.Id))
                .Include(p => p.Category)
                .Include(p => p.Images)
                .ToListAsync();
            return products;
        }

        public async Task<Product?> UpdateAsync(Product entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .SingleOrDefaultAsync(p => p.Id == entity.Id);
        }

        public async Task<bool> ExistsAsync(Product entity)
        {
            return await _products.AnyAsync(e => e.Id == entity.Id);
        }
    }

    internal class ProductImages
    {
    }
}