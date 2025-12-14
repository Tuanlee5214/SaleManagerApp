using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignColors;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Shapes;
using SaleManagerApp.Models;
using MenuItem = SaleManagerApp.Models.MenuItem;
using System.Data;

namespace SaleManagerApp.Services
{
    public class MenuPageService
    {
        private readonly DBConnectionService _db = new DBConnectionService();

        public InsertItemResult InsertMenuItem(MenuItem item)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_InsertMenuItem", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@MenuItemName", System.Data.SqlDbType.NVarChar, 30).Value = item.menuItemName;
                    cmd.Parameters.Add("@UnitPrice", System.Data.SqlDbType.Money).Value = item.unitPrice;
                    cmd.Parameters.Add("@ImageUrl", System.Data.SqlDbType.VarChar, 100).Value = item.imageUrl ?? "";
                    cmd.Parameters.Add("@Size", System.Data.SqlDbType.VarChar, 7).Value = item.size ?? "";
                    cmd.Parameters.Add("@SpecialInfo", System.Data.SqlDbType.NVarChar, 75).Value = item.specialInfo ?? "";
                    cmd.Parameters.Add("@Type", System.Data.SqlDbType.NVarChar, 30).Value = item.type ?? "";

                    int row = cmd.ExecuteNonQuery();
                    if(row != 0)
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
            catch(SqlException ex)
            {
                return new InsertItemResult
                {
                    Success = false,
                    ErrorMessage = "Lỗi kết nối tới server"
                };
            }
        }

        //  Insert khách hàng
        public InsertCustomerResult InsertCustomer(Customer item)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("sp_InsertCustomer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 30).Value = item.fullName;
                    cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 25).Value = item.phone;
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 30).Value = item.email;
                    cmd.Parameters.Add("@Location", SqlDbType.NVarChar, 100).Value = item.address;

                    int row = cmd.ExecuteNonQuery();

                    if (row != 0)
                    {
                        return new InsertCustomerResult
                        {
                            Success = true,
                            SuccessMessage = "Thêm khách hàng thành công"
                        };
                    }
                    else
                    {
                        return new InsertCustomerResult
                        {
                            Success = false,
                            ErrorMessage = "Thêm khách hàng thất bại"
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return new InsertCustomerResult
                {
                    Success = false,
                    ErrorMessage = "Lỗi kết nối tới server",
                };

            }
        }

        public UpdateTableStatus UpdateTableStatus(string tableId, string status)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE [Table] SET tableStatus = @status WHERE tableId = @id";


                    cmd.Parameters.Add("@status", SqlDbType.NVarChar, 30).Value = status;
                    cmd.Parameters.Add("@id", SqlDbType.Char, 7).Value = tableId;

                    int row = cmd.ExecuteNonQuery();

                    if (row != 0)
                    {
                        return new UpdateTableStatus
                        {
                            Success = true,
                            SuccessMessage = "Sửa trạng thái bàn thành công"
                        };
                    }
                    else
                    {
                        return new UpdateTableStatus
                        {
                            Success = false,
                            ErrorMessage = "Sửa trạng thái bàn thất bại"
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return new UpdateTableStatus
                {
                    Success = false,
                    ErrorMessage = "Lỗi kết nối tới server",
                };

            }
        }

        public GetMenuItemsResult GetMenuItems(string type, string searchText)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (var cmd = conn.CreateCommand())
                {

                    cmd.CommandText =
                        "SELECT menuItemId, menuItemName, unitPrice, imageUrl, size, specialInfo, description, type " +
                        "FROM MenuItem " +
                        "WHERE (@type IS NULL OR type = @type) " +
                        "AND (@search IS NULL OR menuItemName LIKE '%' + @search + '%')";

                    // Nếu null => truyền DBNull.Value để WHERE hoạt động đúng
                    cmd.Parameters.Add("@type", SqlDbType.NVarChar, 30)
                        .Value = (object)type ?? DBNull.Value;

                    cmd.Parameters.Add("@search", SqlDbType.NVarChar, 30)
                        .Value = (object)searchText ?? DBNull.Value;

                    var list = new List<MenuItem>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new MenuItem
                            {
                                menuItemId = reader["menuItemId"].ToString(),
                                menuItemName = reader["menuItemName"].ToString(),
                                unitPrice = (decimal)reader["unitPrice"],
                                imageUrl = reader["imageUrl"].ToString(),
                                size = reader["size"].ToString(),
                                specialInfo = reader["specialInfo"].ToString(),
                                description = reader["description"].ToString(),
                                type = reader["type"].ToString()
                            });
                        }
                    }

                    if (list.Count == 0)
                    {
                        return new GetMenuItemsResult
                        {
                            Success = true,
                            MenuItemList = list
                        };
                    }

                    return new GetMenuItemsResult
                    {
                        Success = true,
                        MenuItemList = list
                    };
                }
            }
            catch (SqlException)
            {
                return new GetMenuItemsResult
                {
                    Success = false,
                    ErrorMessage = "Lỗi kết nối tới server"
                };
            }
        }

        public GetTableResult GetTable()
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (var cmd = conn.CreateCommand())
                {

                    cmd.CommandText =
                        "SELECT tableId, tableName, [location], seatCount, tableStatus " +
                        "FROM [Table] ";

                    var list = new List<Table>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Table
                            {
                                tableId = reader["tableId"].ToString(),
                                tableName = reader["tableName"].ToString(),
                                location = reader["location"].ToString(),
                                seatCount = (int)reader["seatCount"],
                                tableStatus = reader["tableStatus"].ToString()
                            });
                        }
                    }

                    if (list.Count == 0)
                    {
                        return new GetTableResult
                        {
                            Success = true,
                            TableList = list
                        };
                    }

                    return new GetTableResult
                    {
                        Success = true,
                        TableList = list
                    };
                }
            }
            catch (SqlException)
            {
                return new GetTableResult
                {
                    Success = false,
                    ErrorMessage = "Lỗi kết nối tới server"
                };
            }
        }


    }
}   


public class InsertItemResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public string SuccessMessage { get; set; }
}

public class GetMenuItemsResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public string SuccessMessage { get; set; }

    public List<MenuItem> MenuItemList;
}

public class GetTableResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public string SuccessMessage { get; set; }

    public  List<Table> TableList;
}

public class InsertCustomerResult
{
    public bool Success { get; set; }
    public string SuccessMessage { get; set; }
    public string ErrorMessage { get; set; }
}
public class UpdateTableStatus
{
    public bool Success { get; set; }
    public string SuccessMessage { get; set; }
    public string ErrorMessage { get; set; }
}

