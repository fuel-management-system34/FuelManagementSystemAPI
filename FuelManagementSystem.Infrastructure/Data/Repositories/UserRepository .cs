using FuelManagementSystem.Core.Entities;
using FuelManagementSystem.Core.Interfaces.Repositiries;
using FuelManagementSystem.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuelManagementSystem.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FuelManagementDbContext _context;

        public UserRepository(FuelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM [User] WHERE UserId = @p0";
            return await _context.Users
                .FromSqlRaw(sql, id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var sql = "SELECT * FROM [User] WHERE Email = @email";
            var entity  = await _context.Users
                .FromSqlRaw(sql, new Microsoft.Data.SqlClient.SqlParameter("@email", email))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return entity;
        }


        public async Task<User> GetByUsernameAsync(string username)
        {
            var sql = "SELECT * FROM [User] WHERE Username = @p0";
            return await _context.Users
                .FromSqlRaw(sql, username)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var sql = "SELECT * FROM [User]";
            return await _context.Users
                .FromSqlRaw(sql)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
        {
            var sql = @"
                SELECT u.* FROM [User] u
                INNER JOIN UserRole ur ON u.UserId = ur.UserId
                WHERE ur.RoleId = @p0";
            return await _context.Users
                .FromSqlRaw(sql, roleId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            var sql = @"
                INSERT INTO [User] (Username, Email, IsActive) 
                VALUES (@p0, @p1, @p2); 
                SELECT * FROM [User] WHERE Email = @p1";

            // NOTE: You can return the inserted user or get the last inserted ID if needed
            return await _context.Users
                .FromSqlRaw(sql, user.Username, user.Email, user.IsActive)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(User user)
        {
            var sql = @"
                UPDATE [User] 
                SET Username = @p0, Email = @p1, IsActive = @p2 
                WHERE UserId = @p3";
            await _context.Database.ExecuteSqlRawAsync(sql, user.Username, user.Email, user.IsActive, user.UserId);
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM [User] WHERE UserId = @p0";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var sql = "SELECT COUNT(*) FROM [User] WHERE UserId = @p0";
            var count = await _context.Users
                .FromSqlRaw("SELECT * FROM [User] WHERE UserId = @p0", id)
                .CountAsync();
            return count > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var count = await _context.Users
                .FromSqlRaw("SELECT * FROM [User] WHERE Email = @p0", email)
                .CountAsync();
            return count > 0;
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            var count = await _context.Users
                .FromSqlRaw("SELECT * FROM [User] WHERE Username = @p0", username)
                .CountAsync();
            return count > 0;
        }
    }
}
