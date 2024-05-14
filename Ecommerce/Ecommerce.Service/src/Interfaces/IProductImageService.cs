using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;

namespace Ecommerce.Service.src.Interfaces
{
    public interface IProductImageService : IBaseService<ProductImageReadDto, ProductImageCreateDto, ProductImageUpdateDto, QueryOptions>
    {

    }
}