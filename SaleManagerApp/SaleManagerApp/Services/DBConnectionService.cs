using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.Services
{
    public class DBConnectionService
    {
        private readonly string _connectString =
            "Server=TUANLEE\\SQLEXPRESS;Database=SaleManagement2025_11;Trusted_Connection=True;";

        public SqlConnection GetConnection()
        {
            var conn = new SqlConnection(_connectString);
            conn.Open();
            return conn;
        }
    }
}
