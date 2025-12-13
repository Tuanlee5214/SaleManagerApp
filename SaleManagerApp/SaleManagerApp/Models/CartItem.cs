using System.ComponentModel;

namespace SaleManagerApp.Models
{
    public class CartItem : INotifyPropertyChanged
    {
        public string MenuItemId { get; }
        public string MenuItemName { get; }
        public decimal UnitPrice { get; }

        public string ImageUrl { get; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity == value) return;
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(SubTotal));
            }
        }

        public decimal SubTotal => Quantity * UnitPrice;

        public CartItem(MenuItem item)
        {
            MenuItemId = item.menuItemId;
            MenuItemName = item.menuItemName;
            UnitPrice = item.unitPrice;
            var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            ImageUrl = System.IO.Path.Combine(baseDir, item.imageUrl);
            Quantity = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
