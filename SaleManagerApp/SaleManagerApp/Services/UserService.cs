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
            try
            {
                using (var conn = _db.GetConnection())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT * FROM [User] WHERE userName = @u AND hashedPassword = @p";

                    cmd.Parameters.Add("@u", System.Data.SqlDbType.VarChar, 20).Value = username;
                    cmd.Parameters.Add("@p", System.Data.SqlDbType.VarChar, 100).Value = PasswordHasher.Hash(password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return new LoginResult
                            {
                                Success = false,
                                ErrorMessage = "Tài khoản hoặc mật khẩu không đúng"
                            };
                        }

                        var user = new User
                        {
                            userId = reader["userId"].ToString(),
                            fullName = reader["fullName"].ToString(),
                            userName = reader["userName"].ToString(),
                            hashedPassword = reader["hashedPassword"].ToString(),
                            avatarUrl = reader["avatarUrl"].ToString(),
                            phone = reader["phone"].ToString(),
                            email = reader["email"].ToString(),
                            groupId = reader["groupId"].ToString(),
                            createdAt = Convert.ToDateTime(reader["createdAt"]),
                            updatedAt = Convert.ToDateTime(reader["updatedAt"])
                        };

                        return new LoginResult
                        {
                            Success = true,
                            SuccesMessage = "Đăng nhập thành công",
                            user = user
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Lỗi kết nối tới máy chủ"
                };
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
