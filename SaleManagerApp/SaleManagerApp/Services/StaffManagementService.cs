using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaleManagerApp.Models;

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
                using (SqlCommand cmd = new SqlCommand("sp_InsertStaffInformation", conn))
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
                    {
                        return new InsertStaffResult
                        {
                            Success = true,
                            SuccessMessage = "Thêm nhân viên thành công"
                        };
                    }
                    else
                    {
                        return new InsertStaffResult
                        {
                            Success = false,
                            ErrorMessage = "Thêm nhân viên không thành công"
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                return new InsertStaffResult
                {
                    Success = false,
                    ErrorMessage = "Lỗi kết nối tới server"
                };
            }
        }

    }
}

public class InsertStaffResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public string SuccessMessage { get; set; }
}
