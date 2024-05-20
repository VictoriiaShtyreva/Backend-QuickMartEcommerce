using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Service.src.Services
{
    public class ReviewService : BaseService<Review, ReviewReadDto, ReviewCreateDto, ReviewUpdateDto, QueryOptions>, IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper, IProductRepository productRepository, IUserRepository userRepository, IMemoryCache cache)
           : base(reviewRepository, mapper, cache)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<ReviewReadDto>> GetReviewsByProductIdAsync(Guid productId)
        {
            var reviews = await _reviewRepository.GetReviewsByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewReadDto>> GetReviewsByUserIdAsync(Guid userId)
        {
            var reviews = await _reviewRepository.GetReviewsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);
        }

        public async Task<ReviewReadDto> CreateReviewAsync(Guid userId, ReviewCreateDto createDto)
        {
            // Validate existence of user and product using their IDs by constructing entities for existence check
            var user = new User { Id = userId };
            var product = new Product { Id = createDto.ProductId };
            // Check for existence
            var userExists = await _userRepository.ExistsAsync(user);
            var productExists = await _productRepository.ExistsAsync(product);

            if (!userExists || !productExists)
            {
                throw AppException.NotFound();
            }
            // Map DTO to Review entity, and set user and product IDs
            var newReview = _mapper.Map<Review>(createDto);
            newReview.UserId = userId;  // Set user ID
            newReview.ProductId = createDto.ProductId;  // Set product ID
            // Add new review to the repository and save changes
            var result = await _reviewRepository.CreateAsync(newReview);
            return _mapper.Map<ReviewReadDto>(result);
        }
    }
}