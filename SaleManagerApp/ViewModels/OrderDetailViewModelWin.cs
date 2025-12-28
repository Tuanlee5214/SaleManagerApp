using SaleManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.ViewModels
{
    public class OrderDetailViewModelWin :BaseViewModel
    {
        public string OrderId { get; set; }
        public string CreatedAt { get; set; }
        public List<MenuItem> Details { get; set; }
        public decimal TotalMoney { get; set; }

        public OrderDetailViewModelWin(RecentOrderItem order, List<MenuItem> details)
        {
            TotalMoney = 0;
            OrderId = order.OrderId;
            Details = details;
            CreatedAt = details[0].createdAt;
            foreach (var c in details)
                TotalMoney += c.total;
        }
    }
}
