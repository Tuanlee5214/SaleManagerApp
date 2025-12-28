using System;
using System.Data.SqlClient;
using System.IO;
using SaleManagerApp.Model;

namespace SaleManagerApp.Services
{
    public class StaffManagementService
    {
        private readonly DBConnectionService _db = new DBConnectionService();

        public Staff GetEmployeeByUserId(string userId)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT e.employeeId, e.fullName, e.dateOfBirth, e.position, 
                           e.phone, e.email, e.imageUrl, e.totalHoursOfMonth, 
                           e.checkInTime, e.workShift, e.createdAt
                    FROM Employee e
                    INNER JOIN [User] u ON e.employeeId = u.employeeId
                    WHERE u.userId = @UserId", conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Staff
                            {
                                StaffId = reader["employeeId"].ToString(),
                                fullName = reader["fullName"].ToString(),
                                dateofBirth = Convert.ToDateTime(reader["dateOfBirth"]).ToString("dd/MM/yyyy"),
                                position = reader["position"].ToString(),
                                phone = reader["phone"] != DBNull.Value ? reader["phone"].ToString() : "",
                                email = reader["email"] != DBNull.Value ? reader["email"].ToString() : "",
                                ImagePath = reader["imageUrl"] != DBNull.Value ? ConvertToAbsolutePath(reader["imageUrl"].ToString()) : null,
                                TotalHoursOfMonth = reader["totalHoursOfMonth"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["totalHoursOfMonth"]) : 0,
                                CheckInTime = reader["checkInTime"] != DBNull.Value
                                    ? (TimeSpan?)((TimeSpan)reader["checkInTime"]) : null,
                                WorkShift = reader["workShift"] != DBNull.Value ? reader["workShift"].ToString() : null,
                                DateStart = Convert.ToDateTime(reader["createdAt"]).ToString("dd/MM/yyyy")
                            };
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetEmployeeByUserId] Error: {ex.Message}");
                return null;
            }
        }

        public InsertStaffResult InsertStaffÌnormation(Staff staff)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_InsertEmployee", conn))
                {
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
                           imageUrl, totalHoursOfMonth, checkInTime, workShift, createdAt 
                    FROM Employee 
                    ORDER BY createdAt DESC", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            staffList.Add(new Staff
                            {
                                StaffId = reader["employeeId"].ToString(),
                                fullName = reader["fullName"].ToString(),
                                dateofBirth = Convert.ToDateTime(reader["dateOfBirth"]).ToString("dd/MM/yyyy"),
                                position = reader["position"].ToString(),
                                phone = reader["phone"] != DBNull.Value ? reader["phone"].ToString() : "",
                                email = reader["email"] != DBNull.Value ? reader["email"].ToString() : "",
                                ImagePath = reader["imageUrl"] != DBNull.Value ? ConvertToAbsolutePath(reader["imageUrl"].ToString()) : null,
                                TotalHoursOfMonth = reader["totalHoursOfMonth"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["totalHoursOfMonth"]) : 0,
                                CheckInTime = reader["checkInTime"] != DBNull.Value
                                    ? (TimeSpan?)((TimeSpan)reader["checkInTime"]) : null,
                                WorkShift = reader["workShift"] != DBNull.Value ? reader["workShift"].ToString() : null,
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

        public GetStaffResult GetStaffByPosition(string position)
        {
            try
            {
                var staffList = new System.Collections.Generic.List<Staff>();

                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_GetStaffByPosition", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Position", System.Data.SqlDbType.VarChar, 20).Value =
                        string.IsNullOrWhiteSpace(position) ? (object)DBNull.Value : position;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            staffList.Add(new Staff
                            {
                                StaffId = reader["employeeId"].ToString(),
                                fullName = reader["fullName"].ToString(),
                                dateofBirth = Convert.ToDateTime(reader["dateOfBirth"]).ToString("dd/MM/yyyy"),
                                position = reader["position"].ToString(),
                                phone = reader["phone"] != DBNull.Value ? reader["phone"].ToString() : "",
                                email = reader["email"] != DBNull.Value ? reader["email"].ToString() : "",
                                ImagePath = reader["imageUrl"] != DBNull.Value ? ConvertToAbsolutePath(reader["imageUrl"].ToString()) : null,
                                TotalHoursOfMonth = reader["totalHoursOfMonth"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["totalHoursOfMonth"]) : 0,
                                CheckInTime = reader["checkInTime"] != DBNull.Value
                                    ? (TimeSpan?)((TimeSpan)reader["checkInTime"]) : null,
                                WorkShift = reader["workShift"] != DBNull.Value ? reader["workShift"].ToString() : null,
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

        private string ConvertToAbsolutePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            try
            {
                // Chuẩn hóa path: thay / thành \
                string normalizedPath = relativePath.Replace("/", "\\");

                // Nếu đã có đuôi file, dùng luôn
                if (Path.HasExtension(normalizedPath))
                {
                    return TryFindFile(normalizedPath);
                }

                // Nếu chưa có đuôi, thử các extension phổ biến
                string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                foreach (string ext in extensions)
                {
                    string pathWithExt = normalizedPath + ext;
                    string foundPath = TryFindFile(pathWithExt);

                    if (foundPath != null)
                        return foundPath;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private string TryFindFile(string path)
        {
            try
            {
                // Nếu là absolute path, check trực tiếp
                if (Path.IsPathRooted(path))
                {
                    return File.Exists(path) ? path : null;
                }

                // Thử các thư mục có thể có
                string[] possiblePaths = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path),
                    Path.Combine(Environment.CurrentDirectory, path),
                    path
                };

                foreach (var testPath in possiblePaths)
                {
                    try
                    {
                        string fullPath = Path.GetFullPath(testPath);
                        if (File.Exists(fullPath))
                            return fullPath;
                    }
                    catch { }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

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
                                string workShift = reader["WorkShift"].ToString();
                                return new AttendanceResult
                                {
                                    Success = true,
                                    Message = message,
                                    CheckInTime = checkInTime,
                                    WorkShift = workShift
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

        public BasicResult UpdateEmployee(Staff staff)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_UpdateEmployee", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@EmployeeId", System.Data.SqlDbType.Char, 7).Value = staff.StaffId;
                    cmd.Parameters.Add("@FullName", System.Data.SqlDbType.NVarChar, 30).Value = staff.fullName;
                    cmd.Parameters.Add("@DateOfBirth", System.Data.SqlDbType.Date).Value = DateTime.Parse(staff.dateofBirth);
                    cmd.Parameters.Add("@Position", System.Data.SqlDbType.VarChar, 20).Value = staff.position;
                    cmd.Parameters.Add("@Phone", System.Data.SqlDbType.VarChar, 25).Value = staff.phone;
                    cmd.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 30).Value = staff.email;
                    cmd.Parameters.Add("@ImageUrl", System.Data.SqlDbType.VarChar, 200).Value =
                        string.IsNullOrWhiteSpace(staff.ImagePath) ? (object)DBNull.Value : staff.ImagePath;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int success = Convert.ToInt32(reader["Success"]);
                            string message = reader["Message"].ToString();

                            return new BasicResult
                            {
                                Success = success == 1,
                                Message = message
                            };
                        }
                    }
                }

                return new BasicResult { Success = false, Message = "Không có phản hồi từ server" };
            }
            catch (Exception ex)
            {
                return new BasicResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public BasicResult ResetMonthlyHours(string employeeId)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_ResetEmployeeMonthlyHours", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmployeeId", System.Data.SqlDbType.Char, 7).Value = employeeId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int success = Convert.ToInt32(reader["Success"]);
                            string message = reader["Message"].ToString();

                            return new BasicResult
                            {
                                Success = success == 1,
                                Message = message
                            };
                        }
                    }
                }

                return new BasicResult { Success = false, Message = "Không có phản hồi từ server" };
            }
            catch (Exception ex)
            {
                return new BasicResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public BasicResult AddNextDaySchedule(string employeeId, string workShift)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_AddNextDaySchedule", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmployeeId", System.Data.SqlDbType.Char, 7).Value = employeeId;
                    cmd.Parameters.Add("@WorkShift", System.Data.SqlDbType.NVarChar, 10).Value = workShift;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int success = Convert.ToInt32(reader["Success"]);
                            string message = reader["Message"].ToString();

                            return new BasicResult
                            {
                                Success = success == 1,
                                Message = message
                            };
                        }
                    }
                }

                return new BasicResult { Success = false, Message = "Không có phản hồi từ server" };
            }
            catch (Exception ex)
            {
                return new BasicResult { Success = false, Message = $"Lỗi: {ex.Message}" };
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
        public string WorkShift { get; set; }
    }

    public class BasicResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}