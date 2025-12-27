using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class TableViewModel : BaseViewModel
    {
        private readonly MenuPageService _service = new MenuPageService();
        public ObservableCollection<Table> Tables { get; }
        = new ObservableCollection<Table>();

        public ICommand SelectTableCommand { get; }


        public TableViewModel()
        {
            SelectTableCommand = new RelayCommand(OnSelectTable);
            LoadTable();
        }


        private void OnSelectTable(object obj)
        {
            if (!(obj is Table table)) return;

            // Xác định trạng thái mới
            string newStatus =
                table.tableStatus == "Còn trống"
                ? "Đã có khách"
                : "Còn trống";

            // Xác nhận
            var result = MessageBox.Show(
                $"Chuyển bàn '{table.tableName}' sang {newStatus.ToUpper()}?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            var updateResult = _service.UpdateTableStatus(table.tableId, newStatus);

            if (!updateResult.Success)
            {
                MessageBox.Show(
                    updateResult.ErrorMessage,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            // Cập nhật UI
            table.tableStatus = newStatus;

            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ToastService.Show(updateResult.SuccessMessage);
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void LoadTable()
        {
            var result = _service.GetTable();
            if (!result.Success) return;

            Tables.Clear();

            foreach (var item in result.TableList)
            {
                Tables.Add(item);
            }

        }
    }
}
