using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.Models
{
    public class Order
    {
        public string orderId { get; set; }
        public string orderStatus { get; set; }
        public string serveStatus { get; set; }
        public string tableId { get; set; }
        public DateTime createdAt { get; set; }

    }

    public class OrderDetail
    {
        public string orderId { get; set; }
        public string menuItemId { get; set; }
        public int quantity { get; set; }
        public decimal currentPrice { get; set; }
        public DateTime createdAt { get; set; }

    }
}
