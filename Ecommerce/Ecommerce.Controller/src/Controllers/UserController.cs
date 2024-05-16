using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;
        public UserController(IUserService userService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }

        [HttpGet("{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserReadDto>> GetUserById([FromRoute] Guid userId)
        {
            var user = await _userService.GetOneByIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, user, "AdminOrOwnerAccountRequirement");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            return Ok(user);
        }

        [HttpGet()]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync([FromQuery] QueryOptions options)
        {
            try
            {
                return await _userService.GetAllAsync(options);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // [HttpPost]
        // [AllowAnonymous]
        // public async Task<ActionResult<UserReadDto>> CreateUser([FromBody] UserCreateDto createDto)
        // {
        //     if (createDto.Avatar != null)
        //     {
        //         var uploadResult = await _imageService.UploadImageAsync(createDto.Avatar);
        //         if (uploadResult.Error != null)
        //         {
        //             return BadRequest(uploadResult.Error.Message);
        //         }
        //         createDto.Avatar = uploadResult.SecureUrl.AbsoluteUri.ToString();
        //     }
        //     var user = await _userService.CreateOneAsync(createDto);
        //     return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, user);
        // }

        [HttpPatch("{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserReadDto>> UpdateUser([FromRoute] Guid userId, UserUpdateDto updateDto)
        {
            var user = await _userService.GetOneByIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, user, "AdminOrOwnerAccountRequirement");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            user = await _userService.UpdateOneAsync(userId, updateDto);
            return Ok(user);
        }

        [HttpPatch("{userId}/reset-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword([FromRoute] Guid userId, [FromBody] string newPassword)
        {
            var user = await _userService.GetOneByIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, user, "AdminOrOwnerAccountRequirement");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            var result = await _userService.ResetPasswordAsync(userId, newPassword);
            return result ? Ok() : NotFound();
        }

        [HttpPatch("{userId}/update-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePassword([FromRoute] Guid userId, [FromBody] string newPassword)
        {
            var user = await _userService.GetOneByIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, user, "AdminOrOwnerAccountRequirement");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            var result = await _userService.UpdatePasswordAsync(userId, newPassword);
            return result ? Ok() : NotFound();
        }

        [HttpPatch("{userId}/update-role")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserReadDto>> UpdateRole([FromRoute] Guid userId, [FromBody] UserRoleUpdateDto roleUpdateDto)
        {
            var user = await _userService.GetOneByIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, user, "AdminOrOwnerAccountRequirement");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            var updatedUser = await _userService.UpdateRoleAsync(userId, roleUpdateDto);
            return Ok(updatedUser);
        }


        [HttpDelete("{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            var user = await _userService.GetOneByIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, user, "AdminOrOwnerAccountRequirement");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            await _userService.DeleteOneAsync(userId);
            return NoContent();
        }
    }

}