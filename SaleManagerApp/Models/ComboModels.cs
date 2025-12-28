using System;
using System.Collections.Generic;

namespace SaleManagerApp.Models
{
    // Model cho khung giờ có hiệu suất thấp
    public class TimeSlotPerformance
    {
        public int DayOfWeek { get; set; }
        public string DayName { get; set; }
        public int HourOfDay { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AvgOrderValue { get; set; }
    }

    // Model cho đơn hàng phục vụ Apriori
    public class OrderItem
    {
        public string OrderId { get; set; }
        public string MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
    }

    // Model cho combo được đề xuất
    public class ComboRecommendation
    {
        public List<string> MenuItemIds { get; set; }
        public List<string> MenuItemNames { get; set; }
        public List<string> ImageUrls { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal ComboPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public double Support { get; set; }
        public double Confidence { get; set; }

        public ComboRecommendation()
        {
            MenuItemIds = new List<string>();
            MenuItemNames = new List<string>();
            ImageUrls = new List<string>();
        }
    }

    // Model cho Combo đã lưu
    public class ComboPromotion
    {
        public string ComboId { get; set; }
        public string ComboName { get; set; }
        public int DayOfWeek { get; set; }
        public string DayName { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal ComboPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public bool IsActive { get; set; }
        public List<ComboDetailItem> Items { get; set; }

        public ComboPromotion()
        {
            Items = new List<ComboDetailItem>();
        }

        // Format hiển thị giờ
        public string TimeSlot => $"{StartHour}:00 - {EndHour}:00";

        // Format hiển thị giá
        public string FormattedPrice => $"{ComboPrice:N0} VNĐ";
        public string FormattedOriginalPrice => $"{OriginalPrice:N0} VNĐ";
    }

    // Model cho món trong combo
    public class ComboDetailItem
    {
        public string MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
    }

    // Model cho việc phân tích itemset (dùng trong Apriori)
    public class ItemSet
    {
        public HashSet<string> Items { get; set; }
        public int Support { get; set; }

        public ItemSet()
        {
            Items = new HashSet<string>();
        }

        public ItemSet(HashSet<string> items)
        {
            Items = new HashSet<string>(items);
        }

        // Override Equals và GetHashCode để so sánh ItemSet
        public override bool Equals(object obj)
        {
            if (obj is ItemSet other)
            {
                return Items.SetEquals(other.Items);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var item in Items)
            {
                hash ^= item.GetHashCode();
            }
            return hash;
        }
    }

    // Model cho quy tắc kết hợp (Association Rule)
    public class AssociationRule
    {
        public HashSet<string> Antecedent { get; set; }  // Món điều kiện (VD: Gà rán)
        public HashSet<string> Consequent { get; set; }  // Món kết quả (VD: Pepsi)
        public double Confidence { get; set; }           // Độ tin cậy
        public double Support { get; set; }              // Độ hỗ trợ
        public double Lift { get; set; }                 // Mức độ liên kết

        public AssociationRule()
        {
            Antecedent = new HashSet<string>();
            Consequent = new HashSet<string>();
        }
    }
}