using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Service.src.Services
{
    public class CategoryService : BaseService<Category, CategoryReadDto, CategoryCreateDto, CategoryUpdateDto, QueryOptions>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IMemoryCache cache)
            : base(categoryRepository, mapper, cache)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<ProductReadDto>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            var products = await _categoryRepository.GetProductsByCategoryIdAsync(categoryId);
            if (products == null || !products.Any()) throw AppException.NotFound();
            return _mapper.Map<IEnumerable<ProductReadDto>>(products);
        }
    }
}