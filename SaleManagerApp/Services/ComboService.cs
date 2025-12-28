using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SaleManagerApp.Models;

namespace SaleManagerApp.Services
{
    /// <summary>
    /// Service layer để ánh xạ dữ liệu từ database sang C# models
    /// Tách biệt logic database khỏi ViewModel
    /// </summary>
    public class ComboService
    {
        private readonly string _connectionString;

        public ComboService(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region READ OPERATIONS - Đọc dữ liệu từ DB

        /// <summary>
        /// Lấy danh sách khung giờ có hiệu suất thấp
        /// Map từ SQL result set sang TimeSlotPerformance model
        /// </summary>
        public async Task<List<TimeSlotPerformance>> GetLowPerformanceTimeSlotsAsync()
        {
            var timeSlots = new List<TimeSlotPerformance>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("sp_GetLowPerformanceTimeSlots", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            timeSlots.Add(MapToTimeSlotPerformance(reader));
                        }
                    }
                }
            }

            return timeSlots;
        }

        /// <summary>
        /// Lấy danh sách items trong đơn hàng cho khung giờ cụ thể
        /// Map sang OrderItem model
        /// </summary>
        public async Task<List<OrderItem>> GetOrderItemsForComboAsync(int dayOfWeek, int startHour, int endHour)
        {
            var items = new List<OrderItem>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("sp_GetOrderItemsForCombo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DayOfWeek", dayOfWeek);
                    cmd.Parameters.AddWithValue("@StartHour", startHour);
                    cmd.Parameters.AddWithValue("@EndHour", endHour);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(MapToOrderItem(reader));
                        }
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Lấy phân bố giá trị đơn hàng
        /// Trả về List<decimal> để tính percentile
        /// </summary>
        public async Task<List<decimal>> GetOrderValueDistributionAsync(int dayOfWeek, int startHour, int endHour)
        {
            var values = new List<decimal>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("sp_GetOrderValueDistribution", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DayOfWeek", dayOfWeek);
                    cmd.Parameters.AddWithValue("@StartHour", startHour);
                    cmd.Parameters.AddWithValue("@EndHour", endHour);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            values.Add(reader.GetDecimal(reader.GetOrdinal("TotalAmount")));
                        }
                    }
                }
            }

            return values;
        }

        /// <summary>
        /// Lấy combo active cho homepage
        /// Map sang ComboPromotion model với đầy đủ chi tiết
        /// </summary>
        public async Task<List<ComboPromotion>> GetActiveCombosAsync()
        {
            var combos = new List<ComboPromotion>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("sp_GetActiveComboForHomepage", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var combo = MapToComboPromotion(reader);
                            combos.Add(combo);
                        }
                    }
                }

                // Load chi tiết món cho mỗi combo
                foreach (var combo in combos)
                {
                    combo.Items = await GetComboDetailsAsync(combo.ComboId);
                }
            }

            return combos;
        }

        /// <summary>
        /// Lấy chi tiết món trong combo
        /// Map sang ComboDetailItem model
        /// </summary>
        public async Task<List<ComboDetailItem>> GetComboDetailsAsync(string comboId)
        {
            var items = new List<ComboDetailItem>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("sp_GetComboDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ComboId", comboId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(MapToComboDetailItem(reader));
                        }
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Lấy tất cả MenuItem để tạo dictionary mapping
        /// Map sang MenuItem model
        /// </summary>
        public async Task<Dictionary<string, MenuItem>> GetMenuItemDictionaryAsync()
        {
            var dict = new Dictionary<string, MenuItem>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand(@"
                    SELECT menuItemId, menuItemName, unitPrice, imageUrl, [type] 
                    FROM MenuItem", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = MapToMenuItem(reader);
                            dict[item.menuItemId] = item;
                        }
                    }
                }
            }

            return dict;
        }

        #endregion

        #region WRITE OPERATIONS - Ghi dữ liệu vào DB

        /// <summary>
        /// Lưu combo mới vào database
        /// Trả về ComboId vừa tạo
        /// </summary>
        public async Task<string> SaveComboPromotionAsync(
            string comboName,
            int dayOfWeek,
            int startHour,
            int endHour,
            decimal discountPercent,
            decimal comboPrice,
            decimal originalPrice)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("sp_SaveComboPromotion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ComboName", comboName);
                    cmd.Parameters.AddWithValue("@DayOfWeek", dayOfWeek);
                    cmd.Parameters.AddWithValue("@StartHour", startHour);
                    cmd.Parameters.AddWithValue("@EndHour", endHour);
                    cmd.Parameters.AddWithValue("@DiscountPercent", discountPercent);
                    cmd.Parameters.AddWithValue("@ComboPrice", comboPrice);
                    cmd.Parameters.AddWithValue("@OriginalPrice", originalPrice);

                    var result = await cmd.ExecuteScalarAsync();
                    return result?.ToString();
                }
            }
        }

        /// <summary>
        /// Thêm món vào combo
        /// </summary>
        public async Task AddItemToComboAsync(string comboId, string menuItemId, int quantity = 1)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("sp_AddItemToCombo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ComboId", comboId);
                    cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Lưu combo hoàn chỉnh (header + details) trong 1 transaction
        /// </summary>
        public async Task<ComboPromotion> SaveCompleteComboAsync(
            string comboName,
            int dayOfWeek,
            int startHour,
            int endHour,
            decimal discountPercent,
            decimal comboPrice,
            decimal originalPrice,
            List<string> menuItemIds)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Lưu combo header
                        string comboId;
                        using (var cmd = new SqlCommand("sp_SaveComboPromotion", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ComboName", comboName);
                            cmd.Parameters.AddWithValue("@DayOfWeek", dayOfWeek);
                            cmd.Parameters.AddWithValue("@StartHour", startHour);
                            cmd.Parameters.AddWithValue("@EndHour", endHour);
                            cmd.Parameters.AddWithValue("@DiscountPercent", discountPercent);
                            cmd.Parameters.AddWithValue("@ComboPrice", comboPrice);
                            cmd.Parameters.AddWithValue("@OriginalPrice", originalPrice);

                            comboId = (await cmd.ExecuteScalarAsync())?.ToString();
                        }

                        if (string.IsNullOrEmpty(comboId))
                        {
                            throw new Exception("Không thể tạo ComboId");
                        }

                        // 2. Lưu combo details
                        foreach (var menuItemId in menuItemIds)
                        {
                            using (var cmd = new SqlCommand("sp_AddItemToCombo", conn, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@ComboId", comboId);
                                cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                                cmd.Parameters.AddWithValue("@Quantity", 1);

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();

                        // 3. Load lại combo vừa tạo để trả về
                        var combo = new ComboPromotion
                        {
                            ComboId = comboId,
                            ComboName = comboName,
                            DayOfWeek = dayOfWeek,
                            StartHour = startHour,
                            EndHour = endHour,
                            DiscountPercent = discountPercent,
                            ComboPrice = comboPrice,
                            OriginalPrice = originalPrice,
                            IsActive = true,
                            Items = await GetComboDetailsAsync(comboId)
                        };

                        return combo;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        #endregion

        #region MAPPING METHODS - Ánh xạ từ DataReader sang Models

        /// <summary>
        /// Map SQL DataReader sang TimeSlotPerformance model
        /// </summary>
        private TimeSlotPerformance MapToTimeSlotPerformance(SqlDataReader reader)
        {
            return new TimeSlotPerformance
            {
                DayOfWeek = reader.GetInt32(reader.GetOrdinal("DayOfWeek")),
                DayName = reader.GetString(reader.GetOrdinal("DayName")),
                HourOfDay = reader.GetInt32(reader.GetOrdinal("HourOfDay")),
                OrderCount = reader.GetInt32(reader.GetOrdinal("OrderCount")),
                TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                AvgOrderValue = reader.GetDecimal(reader.GetOrdinal("AvgOrderValue"))
            };
        }

        /// <summary>
        /// Map SQL DataReader sang OrderItem model
        /// </summary>
        private OrderItem MapToOrderItem(SqlDataReader reader)
        {
            return new OrderItem
            {
                OrderId = reader.GetString(reader.GetOrdinal("orderId")),
                MenuItemId = reader.GetString(reader.GetOrdinal("menuItemId")),
                MenuItemName = reader.GetString(reader.GetOrdinal("menuItemName")),
                UnitPrice = reader.GetDecimal(reader.GetOrdinal("unitPrice")),
                ImageUrl = reader.IsDBNull(reader.GetOrdinal("imageUrl"))
                    ? ""
                    : reader.GetString(reader.GetOrdinal("imageUrl")),
                Type = reader.GetString(reader.GetOrdinal("type")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))
            };
        }

        /// <summary>
        /// Map SQL DataReader sang ComboPromotion model
        /// </summary>
        private ComboPromotion MapToComboPromotion(SqlDataReader reader)
        {
            return new ComboPromotion
            {
                ComboId = reader.GetString(reader.GetOrdinal("comboId")),
                ComboName = reader.GetString(reader.GetOrdinal("comboName")),
                DayOfWeek = reader.GetInt32(reader.GetOrdinal("dayOfWeek")),
                DayName = reader.GetString(reader.GetOrdinal("DayName")),
                StartHour = reader.GetInt32(reader.GetOrdinal("startHour")),
                EndHour = reader.GetInt32(reader.GetOrdinal("endHour")),
                DiscountPercent = reader.GetDecimal(reader.GetOrdinal("discountPercent")),
                ComboPrice = reader.GetDecimal(reader.GetOrdinal("comboPrice")),
                OriginalPrice = reader.GetDecimal(reader.GetOrdinal("originalPrice")),
                IsActive = true,
                Items = new List<ComboDetailItem>() // Sẽ load sau
            };
        }

        /// <summary>
        /// Map SQL DataReader sang ComboDetailItem model
        /// </summary>
        private ComboDetailItem MapToComboDetailItem(SqlDataReader reader)
        {
            return new ComboDetailItem
            {
                MenuItemId = reader.GetString(reader.GetOrdinal("menuItemId")),
                MenuItemName = reader.GetString(reader.GetOrdinal("menuItemName")),
                UnitPrice = reader.GetDecimal(reader.GetOrdinal("unitPrice")),
                ImageUrl = reader.IsDBNull(reader.GetOrdinal("imageUrl"))
                    ? ""
                    : reader.GetString(reader.GetOrdinal("imageUrl")),
                Type = reader.GetString(reader.GetOrdinal("type")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))
            };
        }

        /// <summary>
        /// Map SQL DataReader sang MenuItem model
        /// </summary>
        private MenuItem MapToMenuItem(SqlDataReader reader)
        {
            return new MenuItem
            {
                menuItemId = reader.GetString(reader.GetOrdinal("menuItemId")),
                menuItemName = reader.GetString(reader.GetOrdinal("menuItemName")),
                unitPrice = reader.GetDecimal(reader.GetOrdinal("unitPrice")),
                imageUrl = reader.IsDBNull(reader.GetOrdinal("imageUrl"))
                    ? ""
                    : reader.GetString(reader.GetOrdinal("imageUrl")),
                type = reader.GetString(reader.GetOrdinal("type"))
            };
        }

        #endregion

        #region HELPER METHODS

        /// <summary>
        /// Kiểm tra kết nối database
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi kết nối database: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy tổng số combo active hiện tại
        /// </summary>
        public async Task<int> GetActiveComboCountAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM ComboPromotion 
                    WHERE isActive = 1", conn))
                {
                    return (int)await cmd.ExecuteScalarAsync();
                }
            }
        }

        #endregion
    }
}