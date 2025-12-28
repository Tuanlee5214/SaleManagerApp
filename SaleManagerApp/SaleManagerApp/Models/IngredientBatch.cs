using System;

namespace SaleManagerApp.Models
{
    public class IngredientBatch
    {
        public string BatchId { get; set; }

        public int Quantity { get; set; }

        public DateTime ImportDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        // ===== COMPUTED =====

        public bool IsExpired =>
            ExpiryDate.HasValue &&
            ExpiryDate.Value < DateTime.Today;
    }
}
