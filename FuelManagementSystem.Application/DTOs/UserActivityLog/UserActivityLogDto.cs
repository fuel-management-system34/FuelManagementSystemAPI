using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Application.DTOs.UserActivityLog
{
    public class UserActivityLogDto
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public string IPAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
