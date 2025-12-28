using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SaleManagerApp.Models;

namespace SaleManagerApp.Services
{
    public class WarehouseService
    {
        private readonly DBConnectionService _db = new DBConnectionService();

        // =========================
        // GET INGREDIENT + BATCH
        // =========================
        public GetIngredientsResult GetAllIngredients()
        {
            try
            {
                var ingredients = new Dictionary<string, IngredientItem>();

                using (var conn = _db.GetConnection())
                using (var cmd = new SqlCommand("sp_GetIngredientWithBatch", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var ingredientId = reader["IngredientId"].ToString();

                            if (!ingredients.ContainsKey(ingredientId))
                            {
                                ingredients[ingredientId] = new IngredientItem
                                {
                                    IngredientId = ingredientId,
                                    IngredientName = reader["IngredientName"].ToString(),
                                    Unit = reader["Unit"].ToString(),
                                    MinQuantity = (int)reader["MinQuantity"],

                                    // ===== NEW FIELDS =====
                                    Group = Enum.Parse<IngredientGroup>(
                                        reader["IngredientGroup"].ToString()
                                    ),
                                    ImagePath = reader["ImagePath"]?.ToString()
                                };
                            }

                            // ===== MAP BATCH =====
                            if (reader["BatchId"] != DBNull.Value)
                            {
                                ingredients[ingredientId].Batches.Add(
                                    new IngredientBatch
                                    {
                                        BatchId = reader["BatchId"].ToString(),
                                        Quantity = (int)reader["Quantity"],
                                        ImportDate = (DateTime)reader["ImportDate"],
                                        ExpiryDate = reader["ExpiryDate"] as DateTime?
                                    }
                                );
                            }
                        }
                    }
                }

                return new GetIngredientsResult
                {
                    Success = true,
                    IngredientList = new List<IngredientItem>(ingredients.Values)
                };
            }
            catch (SqlException)
            {
                return new GetIngredientsResult
                {
                    Success = false,
                    ErrorMessage = "Không thể tải dữ liệu kho"
                };
            }
        }

        public CreateIngredientResult CreateIngredient(
    string name,
    string unit,
    IngredientGroup group,
    int minQuantity,
    string imagePath)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (var cmd = new SqlCommand("sp_CreateIngredient", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IngredientName", name);
                    cmd.Parameters.AddWithValue("@Unit", unit);
                    cmd.Parameters.AddWithValue("@IngredientGroup", group.ToString());
                    cmd.Parameters.AddWithValue("@MinQuantity", minQuantity);
                    cmd.Parameters.AddWithValue("@ImagePath",
                        string.IsNullOrWhiteSpace(imagePath)
                            ? (object)DBNull.Value
                            : imagePath);

                    cmd.ExecuteNonQuery();
                }

                return new CreateIngredientResult
                {
                    Success = true,
                    SuccessMessage = "Thêm nguyên liệu thành công"
                };
            }
            catch (SqlException ex)
            {
                // 50002: trùng tên (custom error)
                if (ex.Number == 50002)
                {
                    return new CreateIngredientResult
                    {
                        Success = false,
                        ErrorMessage = "Nguyên liệu đã tồn tại"
                    };
                }

                return new CreateIngredientResult
                {
                    Success = false,
                    ErrorMessage = "Không thể thêm nguyên liệu"
                };
            }
        }

        // =========================
        // IMPORT
        // =========================
        public ImportIngredientResult ImportIngredient(
            string importOrderId,
            string ingredientId,
            int quantity,
            decimal unitPrice,
            DateTime? expiryDate)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (var cmd = new SqlCommand("sp_InsertImportOrderDetail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ImportOrderId", importOrderId);
                    cmd.Parameters.AddWithValue("@IngredientId", ingredientId);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                    cmd.Parameters.AddWithValue("@ExpiryDate",
                        expiryDate.HasValue ? expiryDate.Value : (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                }

                return new ImportIngredientResult
                {
                    Success = true,
                    SuccessMessage = "Nhập kho thành công"
                };
            }
            catch (SqlException)
            {
                return new ImportIngredientResult
                {
                    Success = false,
                    ErrorMessage = "Nhập kho không thành công"
                };
            }
        }

        // =========================
        // EXPORT (FIFO)
        // =========================
        public ExportIngredientResult ExportIngredient(
            string exportOrderId,
            string ingredientId,
            int quantity)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (var cmd = new SqlCommand("sp_ExportIngredientFIFO", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExportOrderId", exportOrderId);
                    cmd.Parameters.AddWithValue("@IngredientId", ingredientId);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    cmd.ExecuteNonQuery();
                }

                return new ExportIngredientResult
                {
                    Success = true,
                    SuccessMessage = "Xuất kho thành công"
                };
            }
            catch (SqlException ex)
            {
                // 50001: không đủ hàng
                if (ex.Number == 50001)
                {
                    return new ExportIngredientResult
                    {
                        Success = false,
                        ErrorMessage = "Số lượng tồn kho không đủ"
                    };
                }

                return new ExportIngredientResult
                {
                    Success = false,
                    ErrorMessage = "Xuất kho không thành công"
                };
            }
        }
    }

    // =========================
    // RESULT CLASSES
    // =========================
    public class GetIngredientsResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<IngredientItem> IngredientList { get; set; }
    }

    public class ImportIngredientResult
    {
        public bool Success { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ExportIngredientResult
    {
        public bool Success { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class CreateIngredientResult
    {
        public bool Success { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
    }

}
