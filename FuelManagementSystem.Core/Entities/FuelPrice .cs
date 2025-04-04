using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Core.Entities
{
    public class FuelPrice : BaseEntity
    {
        public int FuelPriceId { get; set; }
        public int FuelTypeId { get; set; }
        public int StationId { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Navigation properties
        public virtual FuelType FuelType { get; set; }
        public virtual FuelStation Station { get; set; }
    }
}
