using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Core.Entities
{
    public class FuelDelivery : BaseEntity
    {
        public int DeliveryId { get; set; }
        public int StationId { get; set; }
        public int FuelTypeId { get; set; }
        public int TankId { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int? ReceivedById { get; set; }
        public string Notes { get; set; }

        // Navigation properties
        public virtual FuelStation Station { get; set; }
        public virtual FuelType FuelType { get; set; }
        public virtual FuelTank Tank { get; set; }
        public virtual User ReceivedBy { get; set; }
    }
}
