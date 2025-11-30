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

        public LoginResult Login(string username, string password)
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

                cmd.Parameters.Add("@u", System.Data.SqlDbType.VarChar, 20).Value = username;
                cmd.Parameters.Add("@p", System.Data.SqlDbType.VarChar, 100).Value = PasswordHasher.Hash(password);

                reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return new LoginResult { Success = false, ErrorMessage = "Tài khoản hoặc mật khẩu không đúng" };
                }
                else
                {
                    var user = new User();
                    user.userId = reader["userId"].ToString();
                    user.fullName = reader["fullName"].ToString();
                    user.userName = reader["userName"].ToString();
                    user.hashedPassword = reader["hashedPassword"].ToString();
                    user.avatarUrl = reader["avatarUrl"].ToString();
                    user.avatarId = reader["avatarId"].ToString();
                    user.phone = reader["phone"].ToString();
                    user.email = reader["email"].ToString();
                    user.groupId = reader["groupId"].ToString();
                    user.createdAt = Convert.ToDateTime(reader["createdAt"]);
                    user.updatedAt = Convert.ToDateTime(reader["updatedAt"]);

                    Console.WriteLine("user value: " + user.fullName);

                    return new LoginResult { Success = true, SuccesMessage = "Đăng nhập thành công",user = user };
                }
               
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
                return new LoginResult { Success = false, ErrorMessage = "Lỗi kết nối tới máy chủ" };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
                return new LoginResult { Success = false, ErrorMessage = ex.Message };
            }
            finally
            {
                if (reader != null && !reader.IsClosed) reader.Close();
                if (conn != null && conn.State == System.Data.ConnectionState.Open) conn.Close();
            }
        }

    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccesMessage { get; set; }
        public User user { get; set; }
    }
}
