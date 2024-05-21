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
        private readonly ICloudinaryImageService _imageService;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IMemoryCache cache, ICloudinaryImageService imageService)
            : base(categoryRepository, mapper, cache)
        {
            _categoryRepository = categoryRepository;
            _imageService = imageService;
            _cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30));
        }
        public async Task<IEnumerable<ProductReadDto>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            var cacheKey = $"GetProductsByCategory-{categoryId}";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<ProductReadDto>? productsDto))
            {
                var products = await _categoryRepository.GetProductsByCategoryIdAsync(categoryId);
                if (products == null || !products.Any()) throw AppException.NotFound();

                productsDto = _mapper.Map<IEnumerable<ProductReadDto>>(products);
                _cache.Set(cacheKey, productsDto, _cacheOptions);
            }
            return productsDto!;
        }
        public override async Task<CategoryReadDto> CreateOneAsync(CategoryCreateDto createDto)
        {
            var category = new Category();
            category.Name = createDto.Name;
            if (createDto.Image != null)
            {
                var uploadResult = await _imageService.UploadImageAsync(createDto.Image);
                category.Image = uploadResult.SecureUrl.ToString();
            }
            var createdCategory = await _categoryRepository.CreateAsync(category);
            _cache.Remove($"GetAll-{typeof(Category).Name}");
            return _mapper.Map<CategoryReadDto>(createdCategory);
        }

        public override async Task<CategoryReadDto> UpdateOneAsync(Guid id, CategoryUpdateDto updateDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id) ?? throw AppException.NotFound();

            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                existingCategory.Name = updateDto.Name;
            }

            if (updateDto.Image != null)
            {
                if (!string.IsNullOrEmpty(existingCategory.Image))
                {
                    // Extract public ID from the existing image URL to delete it
                    var publicId = new Uri(existingCategory.Image).Segments.Last().Split('.').First();
                    await _imageService.DeleteImageAsync(publicId);
                }
                var uploadResult = await _imageService.UploadImageAsync(updateDto.Image);
                existingCategory.Image = uploadResult.SecureUrl.ToString();
            }

            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
            _cache.Remove($"GetById-{id}");
            _cache.Remove($"GetAll-{typeof(Category).Name}");
            return _mapper.Map<CategoryReadDto>(updatedCategory);
        }


    }
}