using SaleManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SaleManagerApp.Services
{
    public class WarehouseService
    {
        private readonly string _connectionString =
         @"Server=.\SQLEXPRESS;
         Database=SaleManagement20251_12;
         Trusted_Connection=True;
         TrustServerCertificate=True;";

        // ================================================
        // 1. LOAD INGREDIENTS
        // ================================================

        public List<IngredientItem> GetAllIngredients(string filterGroup = null)
        {
            var items = new List<IngredientItem>();

            string query = @"
                SELECT 
                    I.ingredientId,
                    I.ingredientName,
                    I.unit,
                    I.minQuantity,
                    I.[filter],
                    I.maxStorageDays,
                    I.imageUrl
                FROM Ingredient I
                WHERE (@Filter IS NULL OR I.[filter] = @Filter)
                ORDER BY I.ingredientName";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Filter", (object)filterGroup ?? DBNull.Value);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new IngredientItem
                        {
                            IngredientId = reader.GetString(0),
                            IngredientName = reader.GetString(1),
                            Unit = reader.GetString(2),
                            MinQuantity = reader.GetInt32(3),
                            Filter = reader.IsDBNull(4) ? null : reader.GetString(4),
                            MaxStorageDays = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                            ImageUrl = reader.IsDBNull(6) ? null : reader.GetString(6)
                        };

                        items.Add(item);
                    }
                }
            }

            foreach (var item in items)
            {
                var histories = GetBatchHistories(item.IngredientId);
                foreach (var h in histories)
                    item.Histories.Add(h);

                item.Refresh();
            }

            return items;
        }

        public IngredientItem GetIngredientById(string ingredientId)
        {
            IngredientItem item = null;

            string query = @"
                SELECT ingredientId, ingredientName, unit, minQuantity, 
                       [filter], maxStorageDays, imageUrl
                FROM Ingredient
                WHERE ingredientId = @Id";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", ingredientId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = new IngredientItem
                        {
                            IngredientId = reader.GetString(0),
                            IngredientName = reader.GetString(1),
                            Unit = reader.GetString(2),
                            MinQuantity = reader.GetInt32(3),
                            Filter = reader.IsDBNull(4) ? null : reader.GetString(4),
                            MaxStorageDays = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                            ImageUrl = reader.IsDBNull(6) ? null : reader.GetString(6)
                        };
                    }
                }
            }

            if (item != null)
            {
                var histories = GetBatchHistories(ingredientId);
                foreach (var h in histories)
                    item.Histories.Add(h);

                item.Refresh();
            }

            return item;
        }

        // ================================================
        // 2. BATCH HISTORIES
        // ================================================

        public List<IngredientBatchHistory> GetBatchHistories(string ingredientId)
        {
            var list = new List<IngredientBatchHistory>();

            string query = @"
                SELECT 
                    H.historyId,
                    H.ingredientId,
                    H.quantity,
                    H.importDate,
                    H.expiryDate,
                    H.createdAt,
                    H.isDeleted,
                    ISNULL(SUM(WE.quantityExported), 0) AS totalExported
                FROM IngredientBatchHistory H
                LEFT JOIN WarehouseExport WE ON H.historyId = WE.historyId
                WHERE H.ingredientId = @Id
                GROUP BY H.historyId, H.ingredientId, H.quantity, 
                         H.importDate, H.expiryDate, H.createdAt, H.isDeleted
                ORDER BY H.importDate DESC";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", ingredientId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new IngredientBatchHistory
                        {
                            HistoryId = reader.GetString(0),
                            IngredientId = reader.GetString(1),
                            Quantity = reader.GetInt32(2),
                            ImportDate = reader.GetDateTime(3),
                            ExpiryDate = reader.GetDateTime(4),
                            CreatedAt = reader.GetDateTime(5),
                            IsDeleted = reader.GetBoolean(6),
                            QuantityExported = reader.GetInt32(7)
                        });
                    }
                }
            }

            return list;
        }

        // ================================================
        // 3. CREATE INGREDIENT - ✅ FIX
        // ================================================

        public void CreateIngredient(Ingredient ingredient)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string newId = GenerateIngredientId(conn);

                string query = @"
                    INSERT INTO Ingredient (
                        ingredientId, ingredientName, unit, quantity, minQuantity,
                        [filter], maxStorageDays, imageUrl, createdAt, updatedAt
                    )
                    VALUES (
                        @IngredientId, @IngredientName, @Unit, 0, @MinQuantity,
                        @Filter, @MaxStorageDays, @ImageUrl, GETDATE(), GETDATE()
                    )";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IngredientId", newId);
                    cmd.Parameters.AddWithValue("@IngredientName", ingredient.IngredientName);
                    cmd.Parameters.AddWithValue("@Unit", ingredient.Unit);
                    cmd.Parameters.AddWithValue("@MinQuantity", ingredient.MinQuantity);
                    cmd.Parameters.AddWithValue("@Filter", (object)ingredient.Filter ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaxStorageDays", ingredient.MaxStorageDays);
                    cmd.Parameters.AddWithValue("@ImageUrl", (object)ingredient.ImageUrl ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string GenerateIngredientId(SqlConnection conn)
        {
            string query = @"
                SELECT MAX(ingredientId) 
                FROM Ingredient 
                WHERE ingredientId LIKE 'IG%'";

            using (var cmd = new SqlCommand(query, conn))
            {
                var result = cmd.ExecuteScalar();

                if (result == DBNull.Value || result == null)
                {
                    return "IG00001";
                }

                string lastId = result.ToString();
                int number = int.Parse(lastId.Substring(2)) + 1;
                return string.Format("IG{0:D5}", number);
            }
        }

        // ================================================
        // 4. IMPORT - ✅ FIX: Generate historyId
        // ================================================

        public void ImportIngredient(string ingredientId, int quantity,
            DateTime importDate, DateTime expiryDate, string note = null)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // ✅ Generate historyId trước
                string historyId = GenerateHistoryId(conn);

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_ImportIngredientHistory";

                    cmd.Parameters.AddWithValue("@historyId", historyId);
                    cmd.Parameters.AddWithValue("@ingredientId", ingredientId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@importDate", importDate);
                    cmd.Parameters.AddWithValue("@expiryDate", expiryDate);
                    cmd.Parameters.AddWithValue("@note", (object)note ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string GenerateHistoryId(SqlConnection conn)
        {
            string query = @"
                SELECT MAX(historyId) 
                FROM IngredientBatchHistory 
                WHERE historyId LIKE 'IH%'";

            using (var cmd = new SqlCommand(query, conn))
            {
                var result = cmd.ExecuteScalar();

                if (result == DBNull.Value || result == null)
                {
                    return "IH00001";
                }

                string lastId = result.ToString();
                int number = int.Parse(lastId.Substring(2)) + 1;
                return string.Format("IH{0:D5}", number);
            }
        }

        // ================================================
        // 5. UPDATE BATCH
        // ================================================

        public void UpdateBatch(string historyId, int addQuantity,
            DateTime newExpiryDate, string note = null)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_UpdateIngredientBatch";

                    cmd.Parameters.AddWithValue("@historyId", historyId);
                    cmd.Parameters.AddWithValue("@addQuantity", addQuantity);
                    cmd.Parameters.AddWithValue("@newExpiryDate", newExpiryDate);
                    cmd.Parameters.AddWithValue("@note", (object)note ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ================================================
        // 6. EXPORT
        // ================================================

        public void ExportIngredient(string ingredientId, int quantity,
            string employeeId, string note = null)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_ExportIngredient_FEFO";

                    cmd.Parameters.AddWithValue("@ingredientId", ingredientId);
                    cmd.Parameters.AddWithValue("@exportQuantity", quantity);
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    cmd.Parameters.AddWithValue("@note", (object)note ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ================================================
        // 7. DELETE EXPIRED - ✅ FIX
        // ================================================

        public void DeleteExpiredBatch(string historyId, string reason)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // ✅ Soft delete trực tiếp (stored procedure không có sp_DeleteSingleExpiredBatch)
                string query = @"
                    UPDATE IngredientBatchHistory
                    SET isDeleted = 1
                    WHERE historyId = @HistoryId";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@HistoryId", historyId);
                    cmd.ExecuteNonQuery();
                }

                // Ghi log
                string logId = GenerateLogId(conn);
                string logQuery = @"
                    INSERT INTO WarehouseLog (logId, ingredientId, historyId, actionType, quantity, note, createdAt)
                    SELECT @LogId, ingredientId, @HistoryId, N'DELETE', 0, @Reason, GETDATE()
                    FROM IngredientBatchHistory
                    WHERE historyId = @HistoryId";

                using (var cmd = new SqlCommand(logQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@LogId", logId);
                    cmd.Parameters.AddWithValue("@HistoryId", historyId);
                    cmd.Parameters.AddWithValue("@Reason", reason);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string GenerateLogId(SqlConnection conn)
        {
            string query = @"
                SELECT MAX(logId) 
                FROM WarehouseLog 
                WHERE logId LIKE 'WL%'";

            using (var cmd = new SqlCommand(query, conn))
            {
                var result = cmd.ExecuteScalar();

                if (result == DBNull.Value || result == null)
                {
                    return "WL00001";
                }

                string lastId = result.ToString();
                int number = int.Parse(lastId.Substring(2)) + 1;
                return string.Format("WL{0:D5}", number);
            }
        }

        // ================================================
        // 8. WAREHOUSE LOG
        // ================================================

        public List<WarehouseLog> GetWarehouseLogs(int topN = 100)
        {
            var logs = new List<WarehouseLog>();

            string query = @"
                SELECT TOP (@Top)
                    WL.logId,
                    WL.ingredientId,
                    WL.historyId,
                    WL.actionType,
                    WL.quantity,
                    WL.note,
                    WL.createdAt,
                    I.ingredientName,
                    I.unit
                FROM WarehouseLog WL
                JOIN Ingredient I ON WL.ingredientId = I.ingredientId
                ORDER BY WL.createdAt DESC";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Top", topN);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(new WarehouseLog
                        {
                            LogId = reader.GetString(0),
                            IngredientId = reader.GetString(1),
                            HistoryId = reader.IsDBNull(2) ? null : reader.GetString(2),
                            ActionType = reader.GetString(3),
                            Quantity = reader.GetInt32(4),
                            Note = reader.IsDBNull(5) ? null : reader.GetString(5),
                            CreatedAt = reader.GetDateTime(6),
                            IngredientName = reader.GetString(7),
                            Unit = reader.GetString(8)
                        });
                    }
                }
            }

            return logs;
        }
    }
}