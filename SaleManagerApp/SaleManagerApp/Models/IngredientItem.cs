using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SaleManagerApp.Models
{
    public enum IngredientGroup
    {
        Meat,       // Thịt
        Seafood,    // Hải sản
        Vegetable,  // Rau củ
        Spice,      // Gia vị
        Others
    }

    public class IngredientItem : INotifyPropertyChanged
    {
        // ===== MASTER DATA =====
        public string IngredientId { get; set; }
        public string IngredientName { get; set; }

        public string Unit { get; set; }              // kg, g, chai...
        public IngredientGroup Group { get; set; }    // Nhóm nguyên liệu
        public int MinQuantity { get; set; }          // Ngưỡng cảnh báo

        public string ImagePath { get; set; }         // Ảnh (local / url)

        // ===== BATCHES =====
        public ObservableCollection<IngredientBatch> Batches { get; }
            = new ObservableCollection<IngredientBatch>();

        public IngredientItem()
        {
            Batches.CollectionChanged += (_, __) => RefreshStock();
        }

        // ===== COMPUTED PROPERTIES =====

        // Tổng tồn kho
        public int TotalQuantity => Batches.Sum(b => b.Quantity);

        // Sắp hết hàng
        public bool IsLowStock => TotalQuantity <= MinQuantity;

        // Có batch hết hạn
        public bool HasExpiredBatch => Batches.Any(b => b.IsExpired);

        // Batch sắp hết hạn (trong 3 ngày)
        public bool HasNearExpiryBatch =>
            Batches.Any(b => b.ExpiryDate.HasValue &&
                             b.ExpiryDate.Value <= DateTime.Today.AddDays(3));

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Gọi khi batch thay đổi
        public void RefreshStock()
        {
            OnPropertyChanged(nameof(TotalQuantity));
            OnPropertyChanged(nameof(IsLowStock));
            OnPropertyChanged(nameof(HasExpiredBatch));
            OnPropertyChanged(nameof(HasNearExpiryBatch));
        }
    }
}
