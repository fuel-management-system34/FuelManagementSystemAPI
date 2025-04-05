using AutoMapper;
using FuelManagementSystem.Application.DTOs.Role;
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
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public RolesController(
            IRoleService roleService,
            IUserService userService,
            IMapper mapper)
        {
            _roleService = roleService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<RoleDto>>(roles));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<RoleDto>(role));
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto model)
        {
            var role = _mapper.Map<Role>(model);
            var createdRole = await _roleService.CreateRoleAsync(role);

            return CreatedAtAction(nameof(GetById), new { id = createdRole.RoleId }, _mapper.Map<RoleDto>(createdRole));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto model)
        {
            if (id != model.RoleId)
            {
                return BadRequest();
            }

            var role = _mapper.Map<Role>(model);
            var result = await _roleService.UpdateRoleAsync(role);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}/users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersInRole(int id)
        {
            var users = await _userService.GetUsersByRoleAsync(id);
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }
    }
}
