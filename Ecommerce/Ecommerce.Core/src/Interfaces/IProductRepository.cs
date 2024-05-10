using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;

namespace Ecommerce.Core.src.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product, QueryOptions>
    {
        // Get the top purchased products
        Task<IEnumerable<Product>> GetMostPurchasedProductsAsync(int topNumber);

    }
}