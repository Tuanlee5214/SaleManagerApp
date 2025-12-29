using System;

namespace SaleManagerApp.Models
{
    public class IngredientBatchHistory
    {
        // Từ DB
        public string HistoryId { get; set; }
        public string IngredientId { get; set; }
        public int Quantity { get; set; }  // Số lượng nhập ban đầu
        public DateTime ImportDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // COMPUTED từ WarehouseExport
        public int QuantityExported { get; set; }  // Tính từ join với WarehouseExport

        // Computed Properties
        public int RemainingQuantity => Quantity - QuantityExported;

        public bool IsExpired => ExpiryDate.Date < DateTime.Today;

        public bool IsNearExpiry =>
            !IsExpired &&
            ExpiryDate.Date <= DateTime.Today.AddDays(3);

        public bool IsFullyExported => RemainingQuantity <= 0;

        public string Status
        {
            get
            {
                if (IsDeleted) return "Đã xóa";
                if (IsExpired) return "Hết hạn";
                if (IsNearExpiry) return "Sắp hết hạn";
                if (IsFullyExported) return "Đã xuất hết";
                return "Còn hạn";
            }
        }

        // Badge color cho UI
        public string StatusColor
        {
            get
            {
                if (IsDeleted) return "#999";
                if (IsExpired) return "#E74C3C";
                if (IsNearExpiry) return "#F39C12";
                if (IsFullyExported) return "#95A5A6";
                return "#27AE60";
            }
        }
    }
}