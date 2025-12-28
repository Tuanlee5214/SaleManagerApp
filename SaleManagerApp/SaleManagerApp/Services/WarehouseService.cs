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

        // Lấy danh sách nguyên liệu
        public GetIngredientsResult GetAllIngredients()
        {
            try
            {
                var list = new List<Ingredient>();

                using (var conn = _db.GetConnection())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT ingredientId, ingredientName, unit, quantity, minQuantity " +
                        "FROM Ingredient";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Ingredient
                            {
                                ingredientId = reader["ingredientId"].ToString(),
                                ingredientName = reader["ingredientName"].ToString(),
                                unit = reader["unit"].ToString(),
                                quantity = (int)reader["quantity"],
                                minQuantity = (int)reader["minQuantity"]
                            });
                        }
                    }
                }

                return new GetIngredientsResult
                {
                    Success = true,
                    IngredientList = list
                };
            }
            catch (SqlException)
            {
                return new GetIngredientsResult
                {
                    Success = false,
                    ErrorMessage = "Lỗi kết nối tới server"
                };
            }
        }

        // Nhập kho
        public ImportIngredientResult ImportIngredient(string ingredientId, int quantity)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_ImportIngredient", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@IngredientId", SqlDbType.Char, 7).Value = ingredientId;
                    cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = quantity;

                    cmd.ExecuteNonQuery();

                    return new ImportIngredientResult
                    {
                        Success = true,
                        SuccessMessage = "Nhập kho thành công"
                    };
                }
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

        // Xuất kho
        public ExportIngredientResult ExportIngredient(string ingredientId, int quantity)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_ExportIngredient", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@IngredientId", SqlDbType.Char, 7).Value = ingredientId;
                    cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = quantity;

                    cmd.ExecuteNonQuery();

                    return new ExportIngredientResult
                    {
                        Success = true,
                        SuccessMessage = "Xuất kho thành công"
                    };
                }
            }
            catch (SqlException)
            {
                return new ExportIngredientResult
                {
                    Success = false,
                    ErrorMessage = "Xuất kho không thành công"
                };
            }
        }
    }

    // ===== RESULT CLASSES (GIỐNG MENU PAGE) =====

    public class GetIngredientsResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<Ingredient> IngredientList { get; set; }
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
}
