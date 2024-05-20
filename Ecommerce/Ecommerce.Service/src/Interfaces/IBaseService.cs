using Ecommerce.Core.src.Common;

namespace Ecommerce.Service.src.Interfaces
{
    public interface IBaseService<TReadDTO, TCreateDTO, TUpdateDTO, QueryOptions>
    {
        Task<PaginatedResult<TReadDTO>> GetAllAsync(QueryOptions options);
        Task<TReadDTO> GetOneByIdAsync(Guid id);
        Task<TReadDTO> CreateOneAsync(TCreateDTO createDto);
        Task<TReadDTO> UpdateOneAsync(Guid id, TUpdateDTO updateDto);
        Task<bool> DeleteOneAsync(Guid id);
    }
}