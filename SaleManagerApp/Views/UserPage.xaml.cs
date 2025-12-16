using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SaleManagerApp.ViewModels;
using SaleManagerApp.Model;
using SaleManagerApp.Services;

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

        private void CheckInButton_Click(object sender, RoutedEventArgs e)
        {
            // KIỂM TRA QUYỀN TRƯỚC TIÊN
            if (!UserSession.CanManageAttendance())
            {
                MessageBox.Show(
                    "Tài khoản của bạn không có quyền chấm công!\n\n" +
                    "Chỉ Admin hoặc Quản lý mới được phép thực hiện chức năng này.",
                    "Không có quyền",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Kiểm tra đã chọn nhân viên chưa
            var selectedStaff = StaffDataGrid.SelectedItem as Staff;
            if (selectedStaff == null)
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để chấm công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Thực hiện chấm công vào
            _viewModel.CheckIn(selectedStaff.StaffId);
        }

        private void CheckOutButton_Click(object sender, RoutedEventArgs e)
        {
            // KIỂM TRA QUYỀN TRƯỚC TIÊN
            if (!UserSession.CanManageAttendance())
            {
                MessageBox.Show(
                    "Tài khoản của bạn không có quyền chấm công!\n\n" +
                    "Chỉ Admin hoặc Quản lý mới được phép thực hiện chức năng này.",
                    "Không có quyền",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Kiểm tra đã chọn nhân viên chưa
            var selectedStaff = StaffDataGrid.SelectedItem as Staff;
            if (selectedStaff == null)
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để chấm công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // KIỂM TRA: Phải chấm công vào trước
            if (!selectedStaff.CheckInTime.HasValue)
            {
                MessageBox.Show(
                    "Nhân viên chưa chấm công vào!\n\nVui lòng chấm công vào trước khi chấm công ra.",
                    "Không thể chấm công ra",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Thực hiện chấm công ra
            _viewModel.CheckOut(selectedStaff.StaffId);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SearchStaff(SearchBox.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ThemNhanVien();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Logic khi chọn row
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            string employeeId = btn.Tag as string;
            if (string.IsNullOrEmpty(employeeId)) return;

            ContextMenu contextMenu = new ContextMenu
            {
                FontSize = 14,
                MinWidth = 150
            };

            MenuItem editItem = new MenuItem
            {
                Header = "✏️ Sửa thông tin",
                Padding = new Thickness(10, 8, 10, 8)
            };
            editItem.Click += (s, args) =>
            {
                _viewModel.EditStaff(employeeId);
            };

            MenuItem deleteItem = new MenuItem
            {
                Header = "🗑️ Xóa nhân viên",
                Foreground = System.Windows.Media.Brushes.Red,
                Padding = new Thickness(10, 8, 10, 8)
            };
            deleteItem.Click += (s, args) =>
            {
                _viewModel.DeleteStaff(employeeId);
            };

            contextMenu.Items.Add(editItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(deleteItem);

            contextMenu.PlacementTarget = btn;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        }
    }
}