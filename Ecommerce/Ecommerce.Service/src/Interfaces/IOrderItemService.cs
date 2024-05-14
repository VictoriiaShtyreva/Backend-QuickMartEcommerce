using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;

namespace Ecommerce.Service.src.Interfaces
{
    public interface IOrderItemService : IBaseService<OrderItemReadDto, OrderItemCreateDto, OrderItemUpdateDto, QueryOptions>
    {

    }
}