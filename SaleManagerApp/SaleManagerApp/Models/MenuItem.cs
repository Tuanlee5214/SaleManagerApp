using System;
using System.ComponentModel;
using System.Globalization;
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

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }

        public MenuItem()
        {
            IncreaseCommand = new RelayCommand(o =>
            {
                Quantity++;
            });

            DecreaseCommand = new RelayCommand(o =>
            {
                if (Quantity > 0)
                    Quantity--;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
