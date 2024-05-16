using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Service.src.Services
{
    public class ProductService : BaseService<Product, ProductReadDto, ProductCreateDto, ProductUpdateDto, QueryOptions>, IProductService
    {
        private IProductRepository _productRepository;
        private IProductImageRepository _productImageRepository;
        private readonly ICloudinaryImageService _imageService;

        public ProductService(IProductRepository productRepository, IMapper mapper, IProductImageRepository productImageRepository, IMemoryCache cache, ICloudinaryImageService imageService) : base(productRepository, mapper, cache)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _imageService = imageService;

        }

        public async Task<IEnumerable<ProductReadDto>> GetMostPurchased(int topNumber)
        {
            var products = await _productRepository.GetMostPurchasedProductsAsync(topNumber) ?? throw new InvalidOperationException("Unable to fetch the most purchased products.");
            if (!products.Any()) throw AppException.NotFound();
            return _mapper.Map<IEnumerable<ProductReadDto>>(products);
        }

        public override async Task<ProductReadDto> CreateOneAsync(ProductCreateDto createDto)
        {
            var product = new Product(
               title: createDto.Title!,
               description: createDto.Description!,
               price: createDto.Price,
               inventory: createDto.Inventory,
               categoryId: createDto.CategoryId
           );
            var createdProduct = await _productRepository.CreateAsync(product);

            if (createDto.Images != null)
            {
                foreach (var imageFile in createDto.Images)
                {
                    var uploadResult = await _imageService.UploadImageAsync(imageFile);
                    var image = new ProductImage(productId: product.Id, url: uploadResult.SecureUrl.ToString());
                    await _productImageRepository.CreateAsync(image);
                }
            }
            var productReadDto = _mapper.Map<ProductReadDto>(createdProduct);
            return productReadDto;
        }

        public override async Task<ProductReadDto> UpdateOneAsync(Guid id, ProductUpdateDto updateDto)
        {
            var product = await _productRepository.GetByIdAsync(id) ?? throw AppException.NotFound();
            // Update product information if provided in the update DTO
            if (!string.IsNullOrEmpty(updateDto.Title))
            {
                product.Title = updateDto.Title;
            }
            if (updateDto.Price.HasValue)
            {
                product.Price = updateDto.Price.Value;
            }
            if (updateDto.CategoryId != null)
            {
                product.CategoryId = updateDto.CategoryId.Value;
            }
            if (updateDto.Inventory.HasValue)
            {
                product.Inventory = updateDto.Inventory.Value;
            }
            // Update product images
            if (updateDto.Images != null && updateDto.Images.Any())
            {
                foreach (var imageFile in updateDto.Images)
                {
                    var uploadResult = await _imageService.UploadImageAsync(imageFile);
                    var newImage = new ProductImage(productId: product.Id, url: uploadResult.SecureUrl.ToString());
                    await _productImageRepository.CreateAsync(newImage);
                }
            }
            var newProduct = await _repository.UpdateAsync(product);
            return _mapper.Map<ProductReadDto>(newProduct);
        }
    }
}