using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Core.Entities
{
    public class FuelPump : BaseEntity
    {
        [Key]
        public int PumpId { get; set; }
        public int StationId { get; set; }
        public int TankId { get; set; }
        public string PumpNumber { get; set; }
        public int FuelTypeId { get; set; }
        public decimal InitialReading { get; set; }
        public decimal CurrentReading { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public DateTime? InstallationDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual FuelStation Station { get; set; }
        public virtual FuelTank Tank { get; set; }
        public virtual FuelType FuelType { get; set; }
        public virtual ICollection<FuelSale> FuelSales { get; set; }
        //public virtual ICollection<PumpMaintenance> MaintenanceRecords { get; set; }
    }
}
