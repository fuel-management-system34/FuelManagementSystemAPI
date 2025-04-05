using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Core.Entities
{
    public class FuelSale : BaseEntity
    {
        [Key]
        public int SaleId { get; set; }
        public int StationId { get; set; }
        public int PumpId { get; set; }
        public int? AttendantId { get; set; }
        public int ShiftId { get; set; }
        public int? CustomerId { get; set; }
        public int FuelTypeId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public int PaymentMethodId { get; set; }
        public string TransactionReference { get; set; }
        public decimal LoyaltyPointsEarned { get; set; } = 0;
        public decimal LoyaltyPointsRedeemed { get; set; } = 0;
        public string ReceiptNumber { get; set; }
        public DateTime SaleDate { get; set; }

        // Navigation properties
        public virtual FuelStation Station { get; set; }
        public virtual FuelPump Pump { get; set; }
        public virtual User Attendant { get; set; }
       // public virtual Shift Shift { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual FuelType FuelType { get; set; }
        //public virtual PaymentMethod PaymentMethod { get; set; }
        //public virtual ICollection<SaleTax> Taxes { get; set; }
    }
}
