using System.Windows;
using System.Windows.Controls;
using SaleManagerApp.ViewModels;

namespace SaleManagerApp.Views
{
    public partial class UserPage : UserControl
    {
        private UserPageViewModel _viewModel;

        public UserPage()
        {
            InitializeComponent();
            _viewModel = new UserPageViewModel();
            this.DataContext = _viewModel;
        }

        private void ChamCongButton_Click(object sender, RoutedEventArgs e)
        {
            // Gọi ViewModel để xử lý logic
            _viewModel.ChamCong();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Logic tìm kiếm nếu cần
            _viewModel.SearchStaff(SearchBox.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Logic thêm nhân viên
            _viewModel.ThemNhanVien();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Logic khi chọn row
        }
    }
}