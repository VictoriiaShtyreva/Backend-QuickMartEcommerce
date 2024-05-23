using System.Security.Claims;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controllers
{
    [ApiController]
    [Route("api/v1/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IAuthorizationService _authorizationService;

        public ReviewController(IReviewService reviewService, IAuthorizationService authorizationService)
        {
            _reviewService = reviewService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAllProductsAsync([FromQuery] QueryOptions options)
        {
            var products = await _reviewService.GetAllAsync(options);
            return Ok(products);
        }

        [HttpGet("products/{productId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<ReviewReadDto>>> GetReviewsByProductIdAsync([FromRoute] Guid productId)
        {
            var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
            return Ok(reviews);
        }

        [HttpGet("users/{userId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<ReviewReadDto>>> GetReviewsByUserIdAsync([FromRoute] Guid userId)
        {
            var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
            return Ok(reviews);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ReviewReadDto>> CreateReviewAsync([FromBody] ReviewCreateDto reviewCreateDto)
        {
            var userId = GetUserIdClaim();
            var createdReview = await _reviewService.CreateReviewAsync(userId, reviewCreateDto);
            return CreatedAtAction(nameof(GetReviewsByUserIdAsync), new { userId = createdReview.UserId }, createdReview);
        }

        [HttpPatch("{reviewId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ReviewReadDto>> UpdateReviewAsync([FromRoute] Guid reviewId, [FromBody] ReviewUpdateDto reviewUpdateDto)
        {
            var review = await _reviewService.GetOneByIdAsync(reviewId);
            var userId = GetUserIdClaim();
            reviewUpdateDto.UserId = userId;
            if (review == null || (review.UserId != userId && !IsUserAdmin()))
            {
                return Forbid();
            }
            var updated = await _reviewService.UpdateOneAsync(reviewId, reviewUpdateDto);
            if (updated == null) return BadRequest();
            return Ok(updated);
        }

        [HttpDelete("{reviewId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteReviewAsync([FromRoute] Guid reviewId)
        {
            var review = await _reviewService.GetOneByIdAsync(reviewId);
            var userId = GetUserIdClaim();
            if (review == null || (review.UserId != userId && !IsUserAdmin()))
            {
                return Forbid();
            }
            var deleted = await _reviewService.DeleteOneAsync(reviewId);
            if (!deleted) return BadRequest();
            return Ok(deleted);
        }

        private Guid GetUserIdClaim()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("User ID claim not found");
            }
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new Exception("Invalid user ID format");
            }
            return userId;
        }

        private bool IsUserAdmin()
        {
            var userRoleClaim = User.FindFirst(ClaimTypes.Role);
            return userRoleClaim != null && userRoleClaim.Value == UserRole.Admin.ToString();
        }
    }

}
