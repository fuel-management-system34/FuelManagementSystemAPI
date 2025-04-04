using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Core.Entities
{
    public class FuelTank : BaseEntity
    {
        public int TankId { get; set; }
        public int StationId { get; set; }
        public int FuelTypeId { get; set; }
        public string Name { get; set; }
        public decimal Capacity { get; set; }
        public decimal CurrentLevel { get; set; }
        public decimal MinimumLevel { get; set; }
        public DateTime? LastFilledDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual FuelStation Station { get; set; }
        public virtual FuelType FuelType { get; set; }
        public virtual ICollection<FuelDelivery> FuelDeliveries { get; set; }
        public virtual ICollection<FuelPump> FuelPumps { get; set; }
    }
}
