using System;

namespace SaleManagerApp.Models
{
    public class WarehouseLog
    {
        // Từ DB
        public string LogId { get; set; }
        public string IngredientId { get; set; }
        public string HistoryId { get; set; }
        public string ActionType { get; set; }  // IMPORT/UPDATE/EXPORT/DELETE
        public int Quantity { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation (load riêng nếu cần)
        public string IngredientName { get; set; }
        public string Unit { get; set; }

        // UI Properties
        public string ActionIcon
        {
            get
            {
                switch (ActionType)
                {
                    case "IMPORT":
                        return "📥";
                    case "UPDATE":
                        return "✏️";
                    case "EXPORT":
                        return "📤";
                    case "DELETE":
                        return "🗑️";
                    default:
                        return "❓";
                }
            }
        }

        public string ActionColor
        {
            get
            {
                switch (ActionType)
                {
                    case "IMPORT":
                        return "#27AE60";
                    case "UPDATE":
                        return "#3498DB";
                    case "EXPORT":
                        return "#E67E22";
                    case "DELETE":
                        return "#E74C3C";
                    default:
                        return "#95A5A6";
                }
            }
        }

        public string ActionDescription
        {
            get
            {
                switch (ActionType)
                {
                    case "IMPORT":
                        return string.Format("Nhập {0} {1}", Quantity, Unit);
                    case "UPDATE":
                        return string.Format("Cập nhật +{0} {1}", Quantity, Unit);
                    case "EXPORT":
                        return string.Format("Xuất {0} {1}", Quantity, Unit);
                    case "DELETE":
                        return "Xóa batch hết hạn";
                    default:
                        return "Không xác định";
                }
            }
        }

        public string TimeDisplay
        {
            get
            {
                return CreatedAt.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }
}
