using Ecommerce.Service.src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controllers
{
    [ApiController]
    [Route("api/v1/productImages")]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _productImageService;
        private readonly ICloudinaryImageService _imageService;

        public ProductImagesController(IProductImageService productImageService, ICloudinaryImageService imageService)
        {
            _productImageService = productImageService;
            _imageService = imageService;
        }

        [HttpDelete("{imageId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProductImageAsync([FromRoute] Guid imageId)
        {
            var image = await _productImageService.GetOneByIdAsync(imageId);
            if (image == null)
            {
                return NotFound();
            }
            var publicId = new Uri(image.Url!).Segments.Last().Split('.').First();
            await _imageService.DeleteImageAsync(publicId);
            var result = await _productImageService.DeleteOneAsync(imageId);
            return Ok(result);
        }
    }
}