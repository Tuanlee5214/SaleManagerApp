using System;

namespace SaleManagerApp.Models
{
    public class WarehouseExport
    {
        public string ExportId { get; set; }
        public string IngredientId { get; set; }
        public string HistoryId { get; set; }
        public int QuantityExported { get; set; }
        public DateTime ExportDate { get; set; }
        public string EmployeeId { get; set; }
        public string Note { get; set; }
    }
}   