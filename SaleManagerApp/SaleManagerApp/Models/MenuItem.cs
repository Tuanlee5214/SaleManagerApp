using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.Models
{
    public class MenuItem
    {
        public string menuItemId { get; set; }
        public string menuItemName { get; set; }
        public decimal unitPrice { get; set; }
        public string imageUrl { get; set; }
        public string size { get; set; }
        public string specialInfo { get; set; }
        public string description { get; set; }
        public string type { get; set; }
    }
}
