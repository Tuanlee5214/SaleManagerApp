using System;

namespace SaleManagerApp.Models
{
    public class Ingredient
    {
        // Từ DB
        public string IngredientId { get; set; }
        public string IngredientName { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }  // KHÔNG DÙNG - chỉ để tương thích DB
        public int MinQuantity { get; set; }

        // Warehouse fields
        public string Filter { get; set; }  // Meat/Seafood/Vegetable/Spice/Others
        public int MaxStorageDays { get; set; }
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}