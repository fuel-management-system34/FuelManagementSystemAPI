using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Core.Entities
{
    public class FuelStation : BaseEntity
    {
        public int StationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LicenseNumber { get; set; }
        public string TaxId { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<FuelTank> FuelTanks { get; set; }
        public virtual ICollection<FuelPump> FuelPumps { get; set; }
        public virtual ICollection<FuelSale> FuelSales { get; set; }
        //public virtual ICollection<Shift> Shifts { get; set; }
        public virtual ICollection<FuelPrice> FuelPrices { get; set; }
    }
}
