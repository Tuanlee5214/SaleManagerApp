using System;
using System.Windows;
using System.Windows.Controls;

namespace SaleManagerApp.Views
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : UserControl
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private void BtnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            // Tính tổng giá trị đơn hàng
            decimal totalPrice = 000000;

            // Mở cửa sổ OrderSavingConfirm (đã sửa namespace)
            SaleManagerApp.OrderSavingConfirm confirmWindow = new SaleManagerApp.OrderSavingConfirm(totalPrice);

            if (confirmWindow.ShowDialog() == true)
            {
                string selectedTable = confirmWindow.SelectedTable;
                string orderType = confirmWindow.SelectedOrderType;

                MessageBox.Show($"Đã lưu đơn hàng!\nBàn: {selectedTable}\nLoại: {orderType}\nTổng: {totalPrice:N0}VND",
                              "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Đã hủy lưu đơn hàng", "Thông báo");
            }

        }
    }
}