using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SaleManagerApp
{
    public partial class TablePicker : Window
    {
        public string SelectedTable { get; private set; }
        private Button currentSelectedButton;

        public TablePicker(string currentTable)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(currentTable))
            {
                SelectedTable = currentTable;
            }
        }

        private void Table_Click(object sender, RoutedEventArgs e)
        {
            Button clickedBtn = sender as Button;
            if (clickedBtn == null) return;

            // Reset button cũ về trạng thái trống (xám nhạt)
            if (currentSelectedButton != null && currentSelectedButton != clickedBtn)
            {
                currentSelectedButton.Background = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                currentSelectedButton.Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136));
            }

            // Đổi button được click sang màu xanh
            clickedBtn.Background = new SolidColorBrush(Color.FromRgb(74, 123, 247));
            clickedBtn.Foreground = Brushes.White;

            currentSelectedButton = clickedBtn;
            SelectedTable = clickedBtn.Tag.ToString();

            // Đóng cửa sổ và trả về kết quả
            this.DialogResult = true;
            this.Close();
        }
    }
}