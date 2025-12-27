using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SaleManagerApp.Models
{
    public class Ingredient : INotifyPropertyChanged
    {
        public string ingredientId { get; set; }
        public string ingredientName { get; set; }

        public int quantity { get; set; }      // tồn kho hiện tại
        public int minQuantity { get; set; }   // mức cảnh báo

        public string unit { get; set; }       // kg, g, chai...

        // Hiển thị trạng thái kho
        public bool IsLowStock => quantity <= minQuantity;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(
            [CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

