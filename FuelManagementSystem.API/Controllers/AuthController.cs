using AutoMapper;
using FuelManagementSystem.Application.DTOs.Auth;
using FuelManagementSystem.Application.DTOs.Role;
using FuelManagementSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public AuthController(
            IAuthService authService,
            IUserService userService,
            IMapper mapper)
        {
            _authService = authService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model.Username, model.Password);

            if (!result.Success)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var user = await _userService.GetByEmailAsync(model.Username);
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

        [HttpPost("google-login")]
        public async Task<ActionResult<TokenResponseDto>> GoogleLogin([FromBody] GoogleLoginDto model)
        {
            var result = await _authService.GoogleLoginAsync(model.Email, model.GoogleToken);

            if (!result.Success)
            {
                return Unauthorized(new { message = "Invalid credentials or user not registered" });
            }

            var user = await _userService.GetByEmailAsync(model.Email);
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest(new { message = "Password and confirmation password do not match" });
            }

            var result = await _authService.RegisterAsync(
                model.Username,
                model.Email,
                model.Password,
                model.FirstName,
                model.LastName);

            if (result)
            {
                return Ok(new { message = "Registration successful" });
            }

            return BadRequest(new { message = "Registration failed. Username or email might already be in use." });
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
            var username = User.Identity.Name;
            var user = await _userService.GetByEmailAsync(username);
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
    }
}
