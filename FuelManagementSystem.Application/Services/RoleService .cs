using FuelManagementSystem.Core.Entities;
using FuelManagementSystem.Core.Interfaces.Repositiries;
using FuelManagementSystem.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            IRoleRepository roleRepository,
            ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<Role> GetByNameAsync(string name)
        {
            return await _roleRepository.GetByNameAsync(name);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role> CreateRoleAsync(Role role)
        {
            try
            {
                // Check if role with same name already exists
                if (await _roleRepository.ExistsByNameAsync(role.Name))
                {
                    throw new Exception("Role with the same name already exists.");
                }

                role.CreatedAt = DateTime.Now;
                role.UpdatedAt = DateTime.Now;
                return await _roleRepository.AddAsync(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role: {RoleName}", role.Name);
                throw;
            }
        }

        public async Task<bool> UpdateRoleAsync(Role role)
        {
            try
            {
                // Check if role exists
                var existingRole = await _roleRepository.GetByIdAsync(role.RoleId);
                if (existingRole == null)
                {
                    return false;
                }

                // If name is changing, check if the new name conflicts with another role
                if (existingRole.Name != role.Name &&
                    await _roleRepository.ExistsByNameAsync(role.Name))
                {
                    throw new Exception("Role with the same name already exists.");
                }

                existingRole.Name = role.Name;
                existingRole.Description = role.Description;
                existingRole.UpdatedAt = DateTime.Now;

                await _roleRepository.UpdateAsync(existingRole);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role: {RoleId}", role.RoleId);
                throw;
            }
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            try
            {
                // Check if role exists
                if (!(await _roleRepository.ExistsAsync(id)))
                {
                    return false;
                }

                // Delete role
                await _roleRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role: {RoleId}", id);
                throw;
            }
        }
    }
}
