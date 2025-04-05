using FuelManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Core.Interfaces.Repositiries
{
    public interface IUserRoleRepository
    {
        Task<UserRole> GetByIdAsync(int id);
        Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId);
        Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId);
        Task<UserRole> AddAsync(UserRole userRole);
        Task UpdateAsync(UserRole userRole);
        Task DeleteAsync(int id);
        Task DeleteByUserAndRoleAsync(int userId, int roleId);
        Task<bool> ExistsAsync(int userId, int roleId);
    }
}
