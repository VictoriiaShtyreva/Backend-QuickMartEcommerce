using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.WebAPI.src.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Category> _categories;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
            _categories = _context.Categories;
        }
        public async Task<Category> CreateAsync(Category entity)
        {
            await _categories.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _categories.FindAsync(id);
            if (category == null)
                return false;
            _categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<Category>> GetAllAsync(QueryOptions options)
        {
            IQueryable<Category> query = _context.Categories;

            // Get total count
            var totalCount = await query.CountAsync();

            // Sorting
            query = SortCategories(query, options.SortBy, options.SortOrder);

            // Pagination
            query = query.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);

            var categories = await query.ToListAsync();

            return new PaginatedResult<Category>(categories, totalCount);
        }
        private IQueryable<Category> SortCategories(IQueryable<Category> query, SortType sortBy, SortOrder sortOrder)
        {
            switch (sortBy)
            {
                case SortType.byName:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(c => c.Name) :
                            query.OrderByDescending(c => c.Name);
                    break;
                default:
                    // Default sorting by name 
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(c => c.Name) :
                            query.OrderByDescending(c => c.Name);
                    break;
            }
            return query;
        }

        public async Task<Category> GetByIdAsync(Guid id)
        {
            return await _categories.FindAsync(id) ?? throw AppException.NotFound();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            var products = _context.Products.Include(p => p.Images).Where(p => p.CategoryId == categoryId);
            return await products.ToListAsync();
        }

        public async Task<Category?> UpdateAsync(Category entity)
        {
            var category = await _context.Categories.FindAsync(entity.Id);
            if (category == null) return null!;
            category.Name = entity.Name;
            category.Image = entity.Image;
            _categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> ExistsAsync(Category entity)
        {
            return await _categories.AnyAsync(e => e.Id == entity.Id);
        }
    }
}