using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Core.Entities
{
    public class Customer : BaseEntity
    {
        public int CustomerId { get; set; }
        public string CustomerType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string TaxId { get; set; }
        public decimal CreditLimit { get; set; } = 0;
        public decimal CurrentBalance { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<FuelSale> FuelSales { get; set; }
       // public virtual ICollection<CustomerLoyalty> LoyaltyMemberships { get; set; }
       // public virtual ICollection<FuelPass> FuelPasses { get; set; }
    }
}
