using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;

namespace Ecommerce.Service.src.Interfaces
{
    public interface ICategoryService : IBaseService<CategoryReadDto, CategoryCreateDto, CategoryUpdateDto, QueryOptions>
    {
        Task<IEnumerable<ProductReadDto>> GetProductsByCategoryIdAsync(Guid categoryId);
    }
}