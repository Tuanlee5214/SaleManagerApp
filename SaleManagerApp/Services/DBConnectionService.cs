using System;
using System.Data.SqlClient;

namespace SaleManagerApp.Services
{
    public class DBConnectionService
    {
        private readonly string _connectString =
            "Server=HP\\SQLEXPRESS;Database=SaleManagement20251_12;Trusted_Connection=True;";

        /// <summary>
        /// Lấy connection đã mở (giữ nguyên method cũ để tương thích)
        /// </summary>
        public SqlConnection GetConnection()
        {
            var conn = new SqlConnection(_connectString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Lấy connection string để dùng cho các service khác
        /// </summary>
        public string GetConnectionString()
        {
            return _connectString;
        }

        /// <summary>
        /// Test kết nối database
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    return conn.State == System.Data.ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi kết nối database: {ex.Message}");
                return false;
            }
        }
    }
}