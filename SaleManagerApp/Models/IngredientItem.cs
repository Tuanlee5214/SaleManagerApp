using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SaleManagerApp.Models
{
    public class IngredientItem : INotifyPropertyChanged
    {
        // Master Data
        public string IngredientId { get; set; }
        public string IngredientName { get; set; }
        public string Unit { get; set; }
        public string Filter { get; set; }
        public int MinQuantity { get; set; }
        public string ImageUrl { get; set; }
        public int MaxStorageDays { get; set; }

        // Batch History
        public ObservableCollection<IngredientBatchHistory> Histories { get; }
            = new ObservableCollection<IngredientBatchHistory>();

        // Computed Properties
        public int TotalQuantity =>
            Histories
                .Where(h => !h.IsDeleted && !h.IsExpired)
                .Sum(h => h.RemainingQuantity);

        public bool IsLowStock => TotalQuantity <= MinQuantity;

        public bool HasExpiredBatch =>
            Histories.Any(h => h.IsExpired && !h.IsDeleted);

        public bool HasNearExpiryBatch =>
            Histories.Any(h => !h.IsDeleted && h.IsNearExpiry);

        public int ActiveBatchCount =>
            Histories.Count(h => !h.IsDeleted && !h.IsExpired && h.RemainingQuantity > 0);

        public IngredientBatchHistory NearestExpiryBatch =>
            Histories
                .Where(h => !h.IsDeleted && !h.IsExpired && h.RemainingQuantity > 0)
                .OrderBy(h => h.ExpiryDate)
                .FirstOrDefault();

        public DateTime? EarliestExpiryDate => NearestExpiryBatch?.ExpiryDate;

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void Refresh()
        {
            OnPropertyChanged(nameof(TotalQuantity));
            OnPropertyChanged(nameof(IsLowStock));
            OnPropertyChanged(nameof(HasExpiredBatch));
            OnPropertyChanged(nameof(HasNearExpiryBatch));
            OnPropertyChanged(nameof(ActiveBatchCount));
            OnPropertyChanged(nameof(NearestExpiryBatch));
            OnPropertyChanged(nameof(EarliestExpiryDate));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}