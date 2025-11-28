using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.Models
{
    public class User
    {
        public string fullName { get; set; }
        public string userName { get; set; }
        public string hashedPassword { get; set; }
        public string role { get; set; }
        public string avatarUrl { get; set; }
        public string avatarId { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string userId { get; set; }

    }
}
