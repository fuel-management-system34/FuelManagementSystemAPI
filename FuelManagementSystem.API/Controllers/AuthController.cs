using AutoMapper;
using FuelManagementSystem.Application.DTOs.Auth;
using FuelManagementSystem.Application.DTOs.Role;
using FuelManagementSystem.Core.Entities;
using FuelManagementSystem.Core.Interfaces.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using System.Security.Claims;

namespace FuelManagementSystem.API.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
            private readonly IAuthService _authService;
            private readonly IUserService _userService;
            private readonly IMapper _mapper;
            private readonly IConfiguration _configuration;

            public AuthController(
                IAuthService authService,
                IUserService userService,
                IMapper mapper,
                IConfiguration configuration)
            {
                _authService = authService;
                _userService = userService;
                _mapper = mapper;
                _configuration = configuration;
            }

            [HttpPost("google-login")]
            public async Task<ActionResult<TokenResponseDto>> GoogleLogin([FromBody] GoogleLoginDto model)
            {
                try
                {
                    // Verify the Google token
                    var payload = await ValidateGoogleTokenAsync(model.GoogleToken);

                    // Check if the email from Google token matches the one sent by client
                    if (payload.Email != model.Email)
                    {
                        return BadRequest(new { message = "Email mismatch in Google token" });
                    }

                    // Check if the user is allowed to access the system
                    var user = await _userService.GetByEmailAsync(model.Email);
                    if (user == null)
                    {
                        return Unauthorized(new { message = "User is not registered in the system. Please contact your administrator." });
                    }

                    // If user is not active, deny access
                    if (!user.IsActive)
                    {
                        return Unauthorized(new { message = "Your account has been deactivated. Please contact your administrator." });
                    }

                    // Authenticate the user
                    var result = await _authService.GoogleLoginAsync(model.Email, model.GoogleToken);

                    if (!result.Success)
                    {
                        return Unauthorized(new { message = "Invalid credentials or user not registered" });
                    }

                    var roles = await _userService.GetUserRolesAsync(user.UserId);

                    var response = new TokenResponseDto
                    {
                        Success = true,
                        Token = result.Token,
                        RefreshToken = result.RefreshToken,
                        Username = user.Username,
                        Email = user.Email,
                        Roles = roles.Select(r => r.Name).ToList(),
                        Expiration = DateTime.UtcNow.AddHours(1) // Should match token expiration in AuthService
                    };

                    return Ok(response);
                }
                catch (InvalidJwtException)
                {
                    return Unauthorized(new { message = "Invalid Google token" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "An error occurred during authentication", error = ex.Message });
                }
            }

            [Authorize]
            [HttpPost("logout")]
            public async Task<IActionResult> Logout()
            {
                var username = User.Identity.Name;
                var result = await _authService.LogoutAsync(username);

                if (result)
                {
                    return Ok(new { message = "Logout successful" });
                }

                return BadRequest(new { message = "Logout failed" });
            }

            [HttpPost("refresh-token")]
            public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenDto model)
            {
                var result = await _authService.RefreshTokenAsync(model.RefreshToken);

                if (!result.Success)
                {
                    return Unauthorized(new { message = "Invalid refresh token" });
                }

                // In a real implementation, you would get the user from the refresh token
                // This is simplified for demonstration purposes
                var username = User.Identity?.Name;
                User user;

                if (!string.IsNullOrEmpty(username))
                {
                    user = await _userService.GetByEmailAsync(username);
                }
                else
                {
                    // Get user from token claims if available
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
                    {
                        return Unauthorized(new { message = "Invalid token" });
                    }
                    user = await _userService.GetByIdAsync(id);
                }

                if (user == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                var roles = await _userService.GetUserRolesAsync(user.UserId);

                var response = new TokenResponseDto
                {
                    Success = true,
                    Token = result.Token,
                    RefreshToken = result.RefreshToken,
                    Username = user.Username,
                    Email = user.Email,
                    Roles = roles.Select(r => r.Name).ToList(),
                    Expiration = DateTime.UtcNow.AddHours(1) // Should match token expiration in AuthService
                };

                return Ok(response);
            }

            [Authorize]
            [HttpGet("validate-token")]
            public IActionResult ValidateToken()
            {
                // If we get here, the token is valid (due to [Authorize] attribute)
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var email = User.FindFirstValue(ClaimTypes.Email);
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                return Ok(new
                {
                    UserId = userId,
                    Username = username,
                    Email = email,
                    Roles = roles
                });
            }

        // In AuthController.cs when validating Google token
        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string token)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { _configuration["Authentication:Google:ClientId"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
            return payload;
        }
    }
    
}
