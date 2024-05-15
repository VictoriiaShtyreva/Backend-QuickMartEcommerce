using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;

namespace Ecommerce.Service.src.Services
{
    public class ProductImageService : BaseService<ProductImage, ProductImageReadDto, ProductImageCreateDto, ProductImageUpdateDto, QueryOptions>, IProductImageService
    {
        private readonly IProductImageRepository _productImageRepository;
        public ProductImageService(IProductImageRepository productImageRepository, IMapper mapper)
            : base(productImageRepository, mapper)
        {
            _productImageRepository = productImageRepository;
        }
    }
}