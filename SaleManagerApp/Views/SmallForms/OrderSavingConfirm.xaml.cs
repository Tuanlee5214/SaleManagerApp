using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SaleManagerApp
{
    public partial class OrderSavingConfirm : Window
    {
        // Properties để lưu thông tin
        public string SelectedTable { get; private set; }
        public string SelectedOrderType { get; private set; }
        public decimal TotalPrice { get; private set; }

        // Flags để kiểm tra đã chọn chưa
        private bool isTableSelected = false;
        private bool isOrderTypeSelected = false;

        // Constructor - nhận tham số totalPrice
        public OrderSavingConfirm(decimal totalPrice)
        {
            InitializeComponent();
            TotalPrice = totalPrice;
            TotalPriceText.Text = $"{totalPrice:N0}VND";
        }

        private void ChangeTable_Click(object sender, RoutedEventArgs e)
        {
            TablePicker tablePicker = new TablePicker(SelectedTable);

            if (tablePicker.ShowDialog() == true)
            {
                SelectedTable = tablePicker.SelectedTable;
                isTableSelected = true;

                // Đổi button chọn bàn sang màu xanh
                BtnSelectTable.Content = SelectedTable;
                BtnSelectTable.Background = new SolidColorBrush(Color.FromRgb(74, 123, 247));
                BtnSelectTable.Foreground = Brushes.White;

                CheckCanSave();
            }
        }

        private void OrderType_Click(object sender, RoutedEventArgs e)
        {
            Button clickedBtn = sender as Button;
            if (clickedBtn == null) return;

            // Reset tất cả buttons về xám
            BtnTaiBan.Background = new SolidColorBrush(Color.FromRgb(224, 224, 224));
            BtnTaiBan.Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102));

            BtnGiaoHang.Background = new SolidColorBrush(Color.FromRgb(224, 224, 224));
            BtnGiaoHang.Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102));

            BtnChoDatBan.Background = new SolidColorBrush(Color.FromRgb(224, 224, 224));
            BtnChoDatBan.Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102));

            // Đổi button được chọn sang xanh
            clickedBtn.Background = new SolidColorBrush(Color.FromRgb(74, 123, 247));
            clickedBtn.Foreground = Brushes.White;

            SelectedOrderType = clickedBtn.Tag.ToString();
            isOrderTypeSelected = true;

            CheckCanSave();
        }

        private void CheckCanSave()
        {
            // Enable nút Lưu khi đã chọn cả bàn VÀ loại đơn hàng
            if (isTableSelected && isOrderTypeSelected)
            {
                BtnSave.IsEnabled = true;
                BtnSave.Background = new SolidColorBrush(Color.FromRgb(74, 123, 247));
                BtnSave.Foreground = Brushes.White;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (isTableSelected && isOrderTypeSelected)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đủ bàn và loại đơn hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}