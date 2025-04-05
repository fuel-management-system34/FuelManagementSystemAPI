using AutoMapper;
using FuelManagementSystem.Application.DTOs.User;
using FuelManagementSystem.Core.Entities;
using FuelManagementSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuelManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(
            IUserService userService,
            IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto model)
        {
            var user = _mapper.Map<User>(model);
            var createdUser = await _userService.CreateUserAsync(user, model.Password, model.RoleIds);

            return CreatedAtAction(nameof(GetById), new { id = createdUser.UserId }, _mapper.Map<UserDto>(createdUser));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto model)
        {
            if (id != model.UserId)
            {
                return BadRequest();
            }

            var user = _mapper.Map<User>(model);
            var result = await _userService.UpdateUserAsync(user, model.RoleIds);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto model)
        {
            if (id != model.UserId)
            {
                return BadRequest();
            }

            var result = await _userService.ChangePasswordAsync(id, model.CurrentPassword, model.NewPassword);

            if (!result)
            {
                return BadRequest(new { message = "Invalid current password" });
            }

            return NoContent();
        }

        [HttpPut("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto model)
        {
            if (id != model.UserId)
            {
                return BadRequest();
            }

            var result = await _userService.ResetPasswordAsync(id, model.NewPassword);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}/roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(int id)
        {
            var roles = await _userService.GetUserRolesAsync(id);
            return Ok(roles.Select(r => r.Name));
        }

        [HttpPost("{userId}/roles/{roleId}")]
        public async Task<IActionResult> AssignRole(int userId, int roleId)
        {
            var result = await _userService.AssignRoleAsync(userId, roleId);

            if (!result)
            {
                return BadRequest(new { message = "Failed to assign role" });
            }

            return NoContent();
        }

        [HttpDelete("{userId}/roles/{roleId}")]
        public async Task<IActionResult> RemoveRole(int userId, int roleId)
        {
            var result = await _userService.RemoveRoleAsync(userId, roleId);

            if (!result)
            {
                return BadRequest(new { message = "Failed to remove role" });
            }

            return NoContent();
        }
    }
}
