using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.Services
{
    public class UserService
    {
        private readonly DBConnectionService _db = new DBConnectionService();

        public User Login(string username, string password)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                conn = _db.GetConnection();
                cmd = new SqlCommand(
                    "SELECT * FROM [User] WHERE userName = @u AND hashedPassword = @p",
                    conn
                );

                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", PasswordHasher.Hash(password));

                reader = cmd.ExecuteReader();
                if (!reader.Read())
                    return null;

                return new User
                {
                    userId = reader["userId"].ToString(),
                    fullName = reader["fullName"].ToString(),
                    userName = reader["userName"].ToString(),
                    hashedPassword = reader["hashedPassword"].ToString(),
                    role = reader["role"].ToString(),
                    avatarUrl = reader["avatarUrl"].ToString(),
                    avatarId = reader["avatarId"].ToString(),
                    phone = reader["phone"].ToString(),
                    email = reader["email"].ToString(),
                    createdAt = Convert.ToDateTime(reader["createdAt"]),
                    updatedAt = Convert.ToDateTime(reader["updatedAt"])
                };
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
                return null;
            }
            finally
            {
                if (reader != null && !reader.IsClosed) reader.Close();
                if (conn != null && conn.State == System.Data.ConnectionState.Open) conn.Close();
            }
        }

    }
}
