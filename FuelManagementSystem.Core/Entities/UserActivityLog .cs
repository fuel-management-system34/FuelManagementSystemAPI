using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Core.Entities
{
    public class UserActivityLog : BaseEntity
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public string IPAddress { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}
