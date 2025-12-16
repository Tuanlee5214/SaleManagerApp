using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.Services
{
    public class ImportWarehouseService
    {
        private readonly DBConnectionService _db = new DBConnectionService();

        public ImportIngredientResult ImportIngredient(ImportIngredientItem item)
        {
            try
            {
                using (var conn = _db.GetConnection())
                using (var tran = conn.BeginTransaction())
                {
                    // 1. Tạo phiếu nhập
                    var cmdImport = new SqlCommand(
                        "sp_InsertImportOrder", conn, tran);

                    cmdImport.CommandType = CommandType.StoredProcedure;
                    cmdImport.Parameters.Add("@Note", SqlDbType.NVarChar, 100)
                        .Value = item.note ?? "";

                    string importOrderId = cmdImport.ExecuteScalar().ToString();

                    // 2. Chi tiết nhập
                    var cmdDetail = new SqlCommand(
                        "sp_InsertImportOrderDetail", conn, tran);

                    cmdDetail.CommandType = CommandType.StoredProcedure;
                    cmdDetail.Parameters.Add("@ImportOrderId", SqlDbType.Char, 7)
                        .Value = importOrderId;
                    cmdDetail.Parameters.Add("@IngredientId", SqlDbType.Char, 7)
                        .Value = item.ingredientId;
                    cmdDetail.Parameters.Add("@Quantity", SqlDbType.Int)
                        .Value = item.quantity;
                    cmdDetail.Parameters.Add("@UnitPrice", SqlDbType.Money)
                        .Value = 0; // kho không cần tiền

                    cmdDetail.ExecuteNonQuery();

                    tran.Commit();

                    return new ImportIngredientResult
                    {
                        Success = true,
                        SuccessMessage = "Nhập kho thành công"
                    };
                }
            }
            catch
            {
                return new ImportIngredientResult
                {
                    Success = false,
                    ErrorMessage = "Lỗi nhập kho"
                };
            }
        }
    }
}

