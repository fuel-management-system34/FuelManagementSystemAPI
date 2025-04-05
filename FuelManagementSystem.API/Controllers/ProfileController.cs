using AutoMapper;
using FuelManagementSystem.Application.DTOs.User;
using FuelManagementSystem.Core.Entities;
using FuelManagementSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FuelManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ProfileController(
            IUserService userService,
            IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userId != model.UserId)
            {
                return BadRequest();
            }

            var user = _mapper.Map<User>(model);
            var result = await _userService.UpdateUserAsync(user, null); // Not updating roles through profile

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userId != model.UserId)
            {
                return BadRequest();
            }

            var result = await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            if (!result)
            {
                return BadRequest(new { message = "Invalid current password" });
            }

            return NoContent();
        }
    }
}
