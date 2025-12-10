using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.Models
{
    public class Staff
    {
        public string fullName { get; set; }

        public string dateofBirth { get; set; }

        public string address { get; set; }

        public string phone { get; set; }

        public string email { get; set; }

        public string groupId { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime updatedAt { get; set; }

        public string StaffId { get; set; }
    }
}
