using FuelManagementSystem.Core.Entities;
using FuelManagementSystem.Core.Interfaces.Repositiries;
using FuelManagementSystem.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Infrastructure.Data.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly FuelManagementDbContext _context;

        public UserRoleRepository(FuelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<UserRole> GetByIdAsync(int id)
        {
            return await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserRoleId == id);
        }

        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId)
        {
            return await _context.UserRoles
                .Include(ur => ur.User)
                .Where(ur => ur.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<UserRole> AddAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
            return userRole;
        }

        public async Task UpdateAsync(UserRole userRole)
        {
            _context.Entry(userRole).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByUserAndRoleAsync(int userId, int roleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int userId, int roleId)
        {
            return await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }
    }

    }
