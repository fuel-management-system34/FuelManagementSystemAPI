using FuelManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Core.Interfaces.Repositiries
{
    public interface IUserActivityLogRepository
    {
        Task<UserActivityLog> GetByIdAsync(int id);
        Task<IEnumerable<UserActivityLog>> GetByUserIdAsync(int userId);
        Task<UserActivityLog> AddAsync(UserActivityLog log);
    }
}
