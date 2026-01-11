using System;

namespace SaleManagerApp.Models
{
    public enum LogType
    {
        Import,
        Export,
        Update,
        Delete
    }

    public class WarehouseLog
    {
        // DB
        public string LogId { get; set; }
        public string IngredientId { get; set; }
        public string HistoryId { get; set; }
        public string ActionType { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }

        // Join
        public string IngredientName { get; set; }
        public string Unit { get; set; }

        // ===== MAP STRING -> ENUM =====
        public LogType Type
        {
            get
            {
                if (ActionType == "IMPORT") return LogType.Import;
                if (ActionType == "EXPORT") return LogType.Export;
                if (ActionType == "UPDATE") return LogType.Update;
                if (ActionType == "DELETE") return LogType.Delete;

                return LogType.Import;
            }
        }

        // ===== UI =====
        public string ActionIcon
        {
            get
            {
                if (ActionType == "IMPORT") return "📥";
                if (ActionType == "EXPORT") return "📤";
                if (ActionType == "UPDATE") return "✏️";
                if (ActionType == "DELETE") return "🗑️";

                return "❓";
            }
        }

        public string ActionColor
        {
            get
            {
                if (ActionType == "IMPORT") return "#27AE60";
                if (ActionType == "EXPORT") return "#E67E22";
                if (ActionType == "UPDATE") return "#3498DB";
                if (ActionType == "DELETE") return "#E74C3C";

                return "#95A5A6";
            }
        }

        public string ActionDescription
        {
            get
            {
                if (ActionType == "IMPORT")
                    return $"Nhập {Quantity} {Unit}";
                if (ActionType == "EXPORT")
                    return $"Xuất {Quantity} {Unit}";
                if (ActionType == "UPDATE")
                    return $"Cập nhật {Quantity} {Unit}";
                if (ActionType == "DELETE")
                    return "Xóa batch hết hạn";

                return "Không xác định";
            }
        }

        public string TimeDisplay
        {
            get { return CreatedAt.ToString("dd/MM/yyyy HH:mm"); }
        }
    }
}
