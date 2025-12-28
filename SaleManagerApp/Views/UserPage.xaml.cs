using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SaleManagerApp.ViewModels;
using SaleManagerApp.Model;
using SaleManagerApp.Helpers;

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

            // Hiển thị placeholder ban đầu
            UpdatePlaceholderVisibility();
        }

        private void CheckInButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStaff = StaffDataGrid.SelectedItem as Staff;
            if (selectedStaff == null)
            {
                ToastService.ShowError("Vui lòng chọn một nhân viên để chấm công!");
                return;
            }

            _viewModel.CheckIn(selectedStaff.StaffId);
        }

        private void CheckOutButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStaff = StaffDataGrid.SelectedItem as Staff;
            if (selectedStaff == null)
            {
                ToastService.ShowError("Vui lòng chọn một nhân viên để chấm công!");
                return;
            }

            if (!selectedStaff.CheckInTime.HasValue)
            {
                ToastService.ShowError("Nhân viên chưa chấm công vào!");
                return;
            }

            _viewModel.CheckOut(selectedStaff.StaffId);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SearchStaff(SearchBox.Text);
            UpdatePlaceholderVisibility();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Ẩn placeholder khi focus
            PlaceholderPanel.Visibility = Visibility.Collapsed;
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Hiển thị lại placeholder nếu text trống
            UpdatePlaceholderVisibility();
        }

        private void UpdatePlaceholderVisibility()
        {
            if (PlaceholderPanel != null)
            {
                PlaceholderPanel.Visibility = string.IsNullOrEmpty(SearchBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ThemNhanVien();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
                MinWidth = 180
            };

            MenuItem editItem = new MenuItem
            {
                Header = "✏️ Chỉnh sửa thông tin",
                Padding = new Thickness(10, 8, 10, 8)
            };
            editItem.Click += (s, args) =>
            {
                _viewModel.EditStaff(employeeId);
            };

            MenuItem resetHoursItem = new MenuItem
            {
                Header = "🔄 Reset giờ làm tháng",
                Padding = new Thickness(10, 8, 10, 8)
            };
            resetHoursItem.Click += (s, args) =>
            {
                _viewModel.ResetMonthlyHours(employeeId);
            };

            MenuItem addScheduleItem = new MenuItem
            {
                Header = "📅 Thêm ca làm ngày mai",
                Padding = new Thickness(10, 8, 10, 8)
            };
            addScheduleItem.Click += (s, args) =>
            {
                _viewModel.AddNextDaySchedule(employeeId);
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
            contextMenu.Items.Add(resetHoursItem);
            contextMenu.Items.Add(addScheduleItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(deleteItem);

            contextMenu.PlacementTarget = btn;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        }

        private void QuanLyButton_Click(object sender, MouseButtonEventArgs e)
        {
            _viewModel.SelectedPosition = "Quản lý";
        }

        private void PhuBepButton_Click(object sender, MouseButtonEventArgs e)
        {
            _viewModel.SelectedPosition = "Phụ bếp";
        }

        private void PhucVuButton_Click(object sender, MouseButtonEventArgs e)
        {
            _viewModel.SelectedPosition = "Phục vụ";
        }

        private void AllButton_Click(object sender, MouseButtonEventArgs e)
        {
            _viewModel.SelectedPosition = null;
        }
    }
}