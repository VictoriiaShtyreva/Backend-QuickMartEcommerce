using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IAuthorizationService _authorizationService;
        public ProductController(IProductService productService, IAuthorizationService authorizationService)
        {
            _productService = productService;
            _authorizationService = authorizationService;
        }

        [HttpGet("{productId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductReadDto>> GetProductById([FromRoute] Guid productId)
        {
            var product = await _productService.GetOneByIdAsync(productId);
            return Ok(product);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAllProducts([FromQuery] QueryOptions options)
        {
            var products = await _productService.GetAllAsync(options);
            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductReadDto>> CreateProduct([FromForm] ProductCreateDto createDto)
        {
            var product = await _productService.CreateOneAsync(createDto);
            return CreatedAtAction(nameof(GetProductById), new { productId = product.Id }, product);
        }

        [HttpPatch("{productId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductReadDto>> UpdateProduct([FromRoute] Guid productId, [FromForm] ProductUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _productService.UpdateOneAsync(productId, updateDto);
            return Ok(product);
        }

        [HttpDelete("{productId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid productId)
        {
            var result = await _productService.DeleteOneAsync(productId);
            return result ? Ok() : NotFound();
        }

        [HttpGet("most-purchased")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetMostPurchasedProducts([FromQuery] int topNumber)
        {
            var products = await _productService.GetMostPurchased(topNumber);
            return Ok(products);
        }




    }
}