using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;

namespace Ecommerce.Service.src.Interfaces
{
    public interface IProductService : IBaseService<ProductReadDto, ProductCreateDto, ProductUpdateDto, QueryOptions>
    {
        Task<IEnumerable<ProductReadDto>> GetMostPurchased(int topNumber);
    }
}