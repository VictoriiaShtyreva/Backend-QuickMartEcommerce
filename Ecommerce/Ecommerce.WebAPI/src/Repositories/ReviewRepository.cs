using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.WebAPI.src.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Review> _reviews;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
            _reviews = context.Reviews;
        }
        public async Task<Review> CreateAsync(Review entity)
        {
            await _reviews.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var review = await _reviews.FindAsync(id);
            if (review == null) return false;
            _reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Review>> GetAllAsync(QueryOptions options)
        {
            IQueryable<Review> query = _context.Reviews.Include(r => r.User).Include(r => r.Product).ThenInclude(p => p!.Category);

            // Sorting
            query = SortReviews(query, options.SortBy, options.SortOrder);
            // Pagination
            query = query.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);

            return await query.ToListAsync();
        }

        private IQueryable<Review> SortReviews(IQueryable<Review> query, SortType sortBy, SortOrder sortOrder)
        {
            switch (sortBy)
            {
                case SortType.byName:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(r => r.User!.Name) :
                            query.OrderByDescending(r => r.User!.Name);
                    break;
                case SortType.byTitle:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(r => r.Product!.Title) :
                            query.OrderByDescending(r => r.Product!.Title);
                    break;
                case SortType.byCategory:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(r => r.Product!.Category!.Name) :
                            query.OrderByDescending(r => r.Product!.Category!.Name);
                    break;
                default:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(r => r.CreatedAt) :
                            query.OrderByDescending(r => r.CreatedAt);
                    break;
            }
            return query;
        }

        public async Task<Review> GetByIdAsync(Guid id)
        {
            return await _reviews.FindAsync(id) ?? throw AppException.NotFound();
        }

        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(Guid productId)
        {
            return await _reviews
                                 .Include(r => r.User)
                                 .Include(r => r.Product)
                                 .Where(r => r.ProductId == productId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId)
        {
            return await _reviews
                                 .Include(r => r.User)
                                 .Include(r => r.Product)
                                 .Where(r => r.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<Review?> UpdateAsync(Review entity)
        {
            var existingReview = await _reviews.FindAsync(entity.Id);
            if (existingReview == null) return null!;
            existingReview.Rating = entity.Rating;
            existingReview.Content = entity.Content;
            _context.Update(existingReview);
            await _context.SaveChangesAsync();
            return existingReview;
        }
    }
}