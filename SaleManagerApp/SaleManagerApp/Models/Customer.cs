using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Customer
{
    public string customerId { get; set; } 
    public string fullName { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
    public string address { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime? updatedAt { get; set; }
}

