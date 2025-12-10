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
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@fullName", System.Data.SqlDbType.NVarChar, 30).Value = staff.;
                    cmd.Parameters.Add("@UnitPrice", System.Data.SqlDbType.Money).Value = item.unitPrice;
                    cmd.Parameters.Add("@ImageUrl", System.Data.SqlDbType.VarChar, 100).Value = item.imageUrl ?? "";
                    cmd.Parameters.Add("@Size", System.Data.SqlDbType.VarChar, 7).Value = item.size ?? "";
                    cmd.Parameters.Add("@SpecialInfo", System.Data.SqlDbType.NVarChar, 75).Value = item.specialInfo ?? "";
                    cmd.Parameters.Add("@Type", System.Data.SqlDbType.NVarChar, 30).Value = item.type ?? "";

                    int row = cmd.ExecuteNonQuery();
                    if (row != 0)
                    {
                        return new InsertItemResult
                        {
                            Success = true,
                            SuccessMessage = "Thêm món thành công"
                        };
                    }
                    else
                    {
                        return new InsertItemResult
                        {
                            Success = false,
                            ErrorMessage = "Thêm món không thành công"
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                return new InsertItemResult
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
