using FuelManagementSystem.Core.Entities;
using FuelManagementSystem.Core.Interfaces.Repositiries;
using FuelManagementSystem.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementSystem.Infrastructure.Data.Repositories
{
    public class UserActivityLogRepository : IUserActivityLogRepository
    {
        private readonly FuelManagementDbContext _context;

        public UserActivityLogRepository(FuelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<UserActivityLog> GetByIdAsync(int id)
        {
            return await _context.UserActivityLogs
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(log => log.LogId == id);
        }

        public async Task<IEnumerable<UserActivityLog>> GetByUserIdAsync(int userId)
        {
            return await _context.UserActivityLogs
                                 .AsNoTracking()
                                 .Where(log => log.UserId == userId)
                                 .OrderByDescending(log => log)
                                 .ToListAsync();
        }

        public async Task<UserActivityLog> AddAsync(UserActivityLog log)
        {
            await _context.UserActivityLogs.AddAsync(log);
            await _context.SaveChangesAsync();
            return log;
        }
    }
}
