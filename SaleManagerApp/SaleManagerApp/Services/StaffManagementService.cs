// ============================================
// FILE: Services/StaffManagementService.cs
// ============================================
using System;
using System.Data.SqlClient;
using System.IO;
using SaleManagerApp.Model;

namespace SaleManagerApp.Services
{
    public class StaffManagementService
    {
        private readonly DBConnectionService _db = new DBConnectionService();

        public InsertStaffResult InsertStaffÌnormation(Staff staff)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_InsertEmployee", conn))
                {
                    cmd.CommandType = System.Data. CommandType.StoredProcedure;
                    cmd.Parameters.Add("@fullName", System.Data.SqlDbType.NVarChar, 30).Value = staff.fullName;
                    cmd.Parameters.Add("@dateOfBirth", System.Data.SqlDbType.Date).Value = staff.dateofBirth;
                    //cmd.Parameters.Add("@position", System.Data.SqlDbType.VarChar, 20).Value = staff.;
                    cmd.Parameters.Add("@email", System.Data.SqlDbType.NVarChar, 30).Value = staff.email;
                    cmd.Parameters.Add("@phone", System.Data.SqlDbType.NVarChar, 20).Value = staff.phone;
                    cmd.Parameters.Add("@employeeId", System.Data.SqlDbType.Char, 7).Value = staff.StaffId;

                    int row = cmd.ExecuteNonQuery();
                    if (row != 0)
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@FullName", System.Data.SqlDbType.NVarChar, 30).Value = staff.fullName ?? "";
                    cmd.Parameters.Add("@DateOfBirth", System.Data.SqlDbType.Date).Value = DateTime.Parse(staff.dateofBirth);
                    cmd.Parameters.Add("@Position", System.Data.SqlDbType.VarChar, 20).Value = staff.position ?? "";
                    cmd.Parameters.Add("@Phone", System.Data.SqlDbType.VarChar, 25).Value =
                        string.IsNullOrWhiteSpace(staff.phone) ? (object)DBNull.Value : staff.phone;
                    cmd.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 30).Value =
                        string.IsNullOrWhiteSpace(staff.email) ? (object)DBNull.Value : staff.email;
                    cmd.Parameters.Add("@ImageUrl", System.Data.SqlDbType.VarChar, 50).Value =
                        string.IsNullOrWhiteSpace(staff.ImagePath) ? (object)DBNull.Value : staff.ImagePath;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string newEmployeeId = reader["employeeId"].ToString();
                            return new InsertStaffResult
                            {
                                Success = true,
                                SuccessMessage = $"Thêm nhân viên thành công! Mã NV: {newEmployeeId}"
                            };
                        }
                        else
                        {
                            return new InsertStaffResult
                            {
                                Success = false,
                                ErrorMessage = "Không thể thêm nhân viên vào hệ thống!"
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new InsertStaffResult
                {
                    Success = false,
                    ErrorMessage = $"Lỗi: {ex.Message}"
                };
            }
        }

        public GetStaffResult GetAllStaff()
        {
            try
            {
                var staffList = new System.Collections.Generic.List<Staff>();

                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT employeeId, fullName, dateOfBirth, position, phone, email, 
                           imageUrl, totalHoursOfMonth, checkInTime, createdAt 
                    FROM Employee 
                    ORDER BY createdAt DESC", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string imageUrl = reader["imageUrl"] != DBNull.Value ? reader["imageUrl"].ToString() : null;

                            System.Diagnostics.Debug.WriteLine("========================================");
                            System.Diagnostics.Debug.WriteLine($"[LoadStaff] Employee: {reader["fullName"]}");
                            System.Diagnostics.Debug.WriteLine($"[LoadStaff] Image URL from DB: '{imageUrl}'");

                            string absolutePath = ConvertToAbsolutePath(imageUrl);
                            System.Diagnostics.Debug.WriteLine($"[LoadStaff] Final absolute path: '{absolutePath}'");

                            if (!string.IsNullOrEmpty(absolutePath))
                            {
                                System.Diagnostics.Debug.WriteLine($"[LoadStaff] File exists check: {File.Exists(absolutePath)}");
                            }
                            System.Diagnostics.Debug.WriteLine("========================================");

                            staffList.Add(new Staff
                            {
                                StaffId = reader["employeeId"].ToString(),
                                fullName = reader["fullName"].ToString(),
                                dateofBirth = Convert.ToDateTime(reader["dateOfBirth"]).ToString("dd/MM/yyyy"),
                                position = reader["position"].ToString(),
                                phone = reader["phone"] != DBNull.Value ? reader["phone"].ToString() : "",
                                email = reader["email"] != DBNull.Value ? reader["email"].ToString() : "",
                                ImagePath = absolutePath,
                                TotalHoursOfMonth = reader["totalHoursOfMonth"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["totalHoursOfMonth"]) : 0,
                                CheckInTime = reader["checkInTime"] != DBNull.Value
                                    ? (TimeSpan?)((TimeSpan)reader["checkInTime"]) : null,
                                DateStart = Convert.ToDateTime(reader["createdAt"]).ToString("dd/MM/yyyy")
                            });
                        }
                    }
                }

                return new GetStaffResult
                {
                    Success = true,
                    StaffList = staffList
                };
            }
            catch (Exception ex)
            {
                return new GetStaffResult
                {
                    Success = false,
                    ErrorMessage = $"Lỗi: {ex.Message}"
                };
            }
        }

        // HÀM CHUYỂN ĐỔI ĐƯỜNG DẪN (IMPROVED V2)
        private string ConvertToAbsolutePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                System.Diagnostics.Debug.WriteLine("[ConvertPath] Input is null or empty");
                return null;
            }

            try
            {
                // Nếu đã là đường dẫn tuyệt đối
                if (Path.IsPathRooted(relativePath))
                {
                    if (File.Exists(relativePath))
                    {
                        System.Diagnostics.Debug.WriteLine($"[ConvertPath] ✓ Absolute path exists: {relativePath}");
                        return relativePath;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[ConvertPath] ✗ Absolute path NOT exists: {relativePath}");
                        return null;
                    }
                }

                // Chuẩn hóa đường dẫn (thay / bằng \)
                string normalizedPath = relativePath.Replace("/", "\\");
                System.Diagnostics.Debug.WriteLine($"[ConvertPath] Normalized: {normalizedPath}");

                // Thử các vị trí có thể (theo thứ tự ưu tiên)
                string[] possiblePaths = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, normalizedPath),
                    Path.Combine(Environment.CurrentDirectory, normalizedPath),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", normalizedPath),
                    normalizedPath
                };

                for (int i = 0; i < possiblePaths.Length; i++)
                {
                    try
                    {
                        string fullPath = Path.GetFullPath(possiblePaths[i]);
                        System.Diagnostics.Debug.WriteLine($"[ConvertPath] Try #{i + 1}: {fullPath}");

                        if (File.Exists(fullPath))
                        {
                            System.Diagnostics.Debug.WriteLine($"[ConvertPath] ✓✓✓ FOUND at #{i + 1}: {fullPath}");
                            return fullPath;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ConvertPath] Error at #{i + 1}: {ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[ConvertPath] ✗✗✗ NOT FOUND anywhere for: {relativePath}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ConvertPath] EXCEPTION: {ex.Message}");
                return null;
            }
        }

        // XÁC THỰC MÃ NHÂN VIÊN
        public bool ValidateEmployeeId(string employeeId)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Employee WHERE employeeId = @EmployeeId", conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        // CHẤM CÔNG VÀO
        public AttendanceResult CheckIn(string employeeId)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_CheckIn", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmployeeId", System.Data.SqlDbType.Char, 7).Value = employeeId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int success = Convert.ToInt32(reader["Success"]);
                            string message = reader["Message"].ToString();

                            if (success == 1)
                            {
                                TimeSpan checkInTime = (TimeSpan)reader["CheckInTime"];
                                return new AttendanceResult
                                {
                                    Success = true,
                                    Message = message,
                                    CheckInTime = checkInTime
                                };
                            }
                            else
                            {
                                return new AttendanceResult
                                {
                                    Success = false,
                                    Message = message
                                };
                            }
                        }

                        return new AttendanceResult
                        {
                            Success = false,
                            Message = "Không thể thực hiện chấm công"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new AttendanceResult
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        // CHẤM CÔNG RA
        public AttendanceResult CheckOut(string employeeId)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_CheckOut", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmployeeId", System.Data.SqlDbType.Char, 7).Value = employeeId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int success = Convert.ToInt32(reader["Success"]);
                            string message = reader["Message"].ToString();

                            if (success == 1)
                            {
                                TimeSpan checkInTime = (TimeSpan)reader["CheckInTime"];
                                TimeSpan checkOutTime = (TimeSpan)reader["CheckOutTime"];
                                decimal workedHours = Convert.ToDecimal(reader["WorkedHours"]);

                                return new AttendanceResult
                                {
                                    Success = true,
                                    Message = message,
                                    CheckInTime = checkInTime,
                                    CheckOutTime = checkOutTime,
                                    WorkedHours = workedHours
                                };
                            }
                            else
                            {
                                return new AttendanceResult
                                {
                                    Success = false,
                                    Message = message
                                };
                            }
                        }

                        return new AttendanceResult
                        {
                            Success = false,
                            Message = "Không thể thực hiện chấm công"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new AttendanceResult
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public DeleteStaffResult DeleteStaff(string employeeId)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_DeleteEmployee", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmployeeId", System.Data.SqlDbType.Char, 7).Value = employeeId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int success = Convert.ToInt32(reader["Success"]);

                            if (success == 1)
                            {
                                return new DeleteStaffResult
                                {
                                    Success = true,
                                    SuccessMessage = "Xóa nhân viên thành công!"
                                };
                            }
                        }

                        return new DeleteStaffResult
                        {
                            Success = false,
                            ErrorMessage = "Không thể xóa nhân viên khỏi hệ thống!"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new DeleteStaffResult
                {
                    Success = false,
                    ErrorMessage = $"Lỗi: {ex.Message}"
                };
            }
        }
    }

    public class InsertStaffResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
    }

    public class GetStaffResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public System.Collections.Generic.List<Staff> StaffList { get; set; }
    }

    public class DeleteStaffResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
    }

    public class AttendanceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public decimal WorkedHours { get; set; }
    }
}