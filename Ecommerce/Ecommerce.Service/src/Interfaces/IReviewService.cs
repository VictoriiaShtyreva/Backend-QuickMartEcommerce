using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;

namespace Ecommerce.Service.src.Interfaces
{
    public interface IReviewService : IBaseService<ReviewReadDto, ReviewCreateDto, ReviewUpdateDto, QueryOptions>
    {
        Task<IEnumerable<ReviewReadDto>> GetReviewsByProductIdAsync(Guid productId);
        Task<IEnumerable<ReviewReadDto>> GetReviewsByUserIdAsync(Guid userId);
        Task<ReviewReadDto> CreateReviewAsync(Guid userId, ReviewCreateDto createDto);
    }
}