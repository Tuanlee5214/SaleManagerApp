using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SaleManagerApp.Models
{
    public class MenuItem : INotifyPropertyChanged
    {
        public string menuItemId { get; set; }
        public string menuItemName { get; set; }
        public decimal unitPrice { get; set; }
        public string imageUrl { get; set; }
        public string size { get; set; }
        public string specialInfo { get; set; }
        public string description { get; set; }
        public string type { get; set; }

        public string imageFullPath
        {
            get
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                return System.IO.Path.Combine(baseDir, imageUrl);
            }
        }

        public string unitPriceDisplay
        {
            get
            {
                return unitPrice.ToString("N0", new CultureInfo("vi-VN"));
            }
        }

        private int _displayQuantity;
        public int DisplayQuantity
        {
            get => _displayQuantity;
            set
            {
                _displayQuantity = value;
                OnPropertyChanged(nameof(DisplayQuantity));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
