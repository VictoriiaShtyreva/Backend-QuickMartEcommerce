using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("{categoryId}")]
        [AllowAnonymous]
        [ActionName(nameof(GetCategoryByIdAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryReadDto>> GetCategoryByIdAsync([FromRoute] Guid categoryId)
        {
            var category = await _categoryService.GetOneByIdAsync(categoryId);
            return Ok(category);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetAllCategoriesAsync([FromQuery] QueryOptions options)
        {
            var categories = await _categoryService.GetAllAsync(options);
            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ActionName(nameof(CreateCategoryAsync))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryReadDto>> CreateCategoryAsync([FromForm] CategoryCreateDto createDto)
        {
            var createdCategory = await _categoryService.CreateOneAsync(createDto);
            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { categoryId = createdCategory.Id }, createdCategory);
        }

        [HttpPatch("{categoryId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryReadDto>> UpdateCategoryAsync([FromRoute] Guid categoryId, [FromForm] CategoryUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedCategory = await _categoryService.UpdateOneAsync(categoryId, updateDto);
            return Ok(updatedCategory);
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCategoryAsync(Guid categoryId)
        {
            var result = await _categoryService.DeleteOneAsync(categoryId);
            return result ? Ok(result) : NotFound();
        }

        [HttpGet("{categoryId}/products")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProductsAsync([FromRoute] Guid categoryId)
        {
            var products = await _categoryService.GetProductsByCategoryIdAsync(categoryId);
            return Ok(products);
        }
    }
}