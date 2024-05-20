using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
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
        public async Task<ActionResult<UserReadDto>> GetUserByIdAsync([FromRoute] Guid userId)
        {
            var user = await _userService.GetOneByIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, user, "AdminOrOwnerAccount");
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
        public async Task<IActionResult> GetAllUsersAsync([FromQuery] QueryOptions options)
        {
            var result = await _userService.GetAllAsync(options);
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<UserReadDto>> CreateUserAsync([FromForm] UserCreateDto createDto)
        {
            var user = await _userService.CreateOneAsync(createDto);
            return CreatedAtAction(nameof(GetUserByIdAsync), new { userId = user.Id }, user);
        }

        [HttpPatch("{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserReadDto>> UpdateUserAsync([FromRoute] Guid userId, [FromForm] UserUpdateDto updateDto)
        {
            var user = await _userService.GetOneByIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, user, "AdminOrOwnerAccount");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var updatedUser = await _userService.UpdateOneAsync(userId, updateDto);
            return Ok(updatedUser);
        }

        // for account recovery scenarios
        [HttpPatch("{userId}/reset-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPasswordAsync([FromRoute] Guid userId, [FromBody] string newPassword)
        {
            var user = await _userService.GetOneByIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, user, "AdminOrOwnerAccount");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            var result = await _userService.ResetPasswordAsync(userId, newPassword);
            return result ? Ok(result) : NotFound();
        }

        [HttpPatch("{userId}/update-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePasswordAsync([FromRoute] Guid userId, [FromBody] string newPassword)
        {
            var user = await _userService.GetOneByIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, user, "AdminOrOwnerAccount");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            var result = await _userService.UpdatePasswordAsync(userId, newPassword);
            return result ? Ok(result) : NotFound();
        }

        [HttpPatch("{userId}/update-role")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserReadDto>> UpdateRoleAsync([FromRoute] Guid userId, [FromBody] UserRoleUpdateDto roleUpdateDto)
        {
            var user = await _userService.GetOneByIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, user, "AdminOrOwnerAccount");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            var updatedUser = await _userService.UpdateRoleAsync(userId, roleUpdateDto);
            return Ok(updatedUser);
        }


        [HttpDelete("{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid userId)
        {
            var user = await _userService.GetOneByIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, user, "AdminOrOwnerAccount");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            var result = await _userService.DeleteOneAsync(userId);
            return Ok(result);
        }
    }

}