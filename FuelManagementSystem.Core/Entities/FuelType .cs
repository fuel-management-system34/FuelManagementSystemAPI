using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Core.Entities
{
    public class FuelType : BaseEntity
    {
        public int FuelTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<FuelTank> FuelTanks { get; set; }
        public virtual ICollection<FuelPump> FuelPumps { get; set; }
        public virtual ICollection<FuelSale> FuelSales { get; set; }
        public virtual ICollection<FuelPrice> FuelPrices { get; set; }
    }
}
