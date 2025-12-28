using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.Models
{
    public class Invoice
    {
        public string invoiceId { get; set; }    
        public string orderId { get; set; }      
        public string paymentMethod { get; set; } 
        public decimal totalAmount { get; set; } 
        public string invoiceStatus { get; set; }
    }
}
