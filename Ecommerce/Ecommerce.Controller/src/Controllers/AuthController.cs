using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<string> LoginAsync([FromBody] UserCredential userCredential)
        {
            return await _authService.LogInAsync(userCredential);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            // Retrieve token from the Authorization header
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            // Check if the token exists
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }
            // Call the logout method
            await _authService.LogoutAsync();
            return Ok("Logged out successfully");
        }

        [HttpGet("authenticate")]
        public async Task<ActionResult<UserReadDto>> AuthenticateAsync([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                if (!Request.Headers.TryGetValue("Authorization", out var bearerToken))
                {
                    return Unauthorized("Missing Authorization Header");
                }
                token = bearerToken.ToString().Replace("Bearer ", "");
            }
            var user = await _authService.AuthenticateUserAsync(token);
            return Ok(user);
        }
    }
}