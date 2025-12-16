using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SaleManagerApp.Model;
using SaleManagerApp.Services;

namespace SaleManagerApp.ViewModels
{
    public class UserPageViewModel : BaseViewModel
    {
        private readonly StaffManagementService _service = new StaffManagementService();
        private ObservableCollection<Staff> _allStaffList;
        private ObservableCollection<Staff> _staffList;

        public ObservableCollection<Staff> StaffList
        {
            get => _staffList;
            set
            {
                _staffList = value;
                OnPropertyChanged(nameof(StaffList));
            }
        }

        public UserPageViewModel()
        {
            _allStaffList = new ObservableCollection<Staff>();
            StaffList = new ObservableCollection<Staff>();
            LoadStaffData();
        }

        private void LoadStaffData()
        {
            var result = _service.GetAllStaff();

            _allStaffList.Clear();

            if (result.Success && result.StaffList != null)
            {
                foreach (var staff in result.StaffList)
                {
                    _allStaffList.Add(staff);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    MessageBox.Show(result.ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            StaffList = new ObservableCollection<Staff>(_allStaffList);
        }

        // CHẤM CÔNG VÀO
        public void CheckIn(string employeeId)
        {
            var result = _service.CheckIn(employeeId);

            if (result.Success)
            {
                ShowAttendanceDialog(true, $"{result.Message}\nThời gian: {result.CheckInTime:hh\\:mm\\:ss}");
                LoadStaffData(); // Reload để cập nhật trạng thái
            }
            else
            {
                ShowAttendanceDialog(false, result.Message);
            }
        }

        // CHẤM CÔNG RA
        public void CheckOut(string employeeId)
        {
            var result = _service.CheckOut(employeeId);

            if (result.Success)
            {
                string message = $"{result.Message}\n" +
                                $"Giờ vào: {result.CheckInTime:hh\\:mm\\:ss}\n" +
                                $"Giờ ra: {result.CheckOutTime:hh\\:mm\\:ss}\n" +
                                $"Số giờ làm: {result.WorkedHours} giờ";
                ShowAttendanceDialog(true, message);
                LoadStaffData(); // Reload để cập nhật số giờ
            }
            else
            {
                ShowAttendanceDialog(false, result.Message);
            }
        }

        private void ShowAttendanceDialog(bool isSuccess, string message)
        {
            Window dialog = new Window
            {
                Width = 450,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                Background = System.Windows.Media.Brushes.Transparent,
                AllowsTransparency = true
            };

            Border border = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                CornerRadius = new CornerRadius(20),
                Padding = new Thickness(30),
            };

            StackPanel stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            TextBlock title = new TextBlock
            {
                Text = isSuccess ? "✓ THÀNH CÔNG" : "✗ THẤT BẠI",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 10, 0, 20),
                Foreground = new System.Windows.Media.SolidColorBrush(
                    isSuccess ?
                        System.Windows.Media.Color.FromRgb(40, 167, 69) :
                        System.Windows.Media.Color.FromRgb(220, 53, 69))
            };

            TextBlock messageBlock = new TextBlock
            {
                Text = message,
                FontSize = 16,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 30),
                Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(17, 24, 39))
            };

            Button backButton = new Button
            {
                Content = "Đóng",
                Width = 200,
                Height = 45,
                Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(64, 123, 255)),
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand,
                BorderThickness = new Thickness(0)
            };

            backButton.Template = new System.Windows.Controls.ControlTemplate(typeof(Button))
            {
                VisualTree = CreateButtonTemplate()
            };

            backButton.Click += (s, e) => dialog.Close();

            stackPanel.Children.Add(title);
            stackPanel.Children.Add(messageBlock);
            stackPanel.Children.Add(backButton);

            border.Child = stackPanel;
            dialog.Content = border;

            dialog.ShowDialog();
        }

        private System.Windows.FrameworkElementFactory CreateButtonTemplate()
        {
            var borderFactory = new System.Windows.FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(22));
            borderFactory.SetValue(Border.BackgroundProperty,
                new System.Windows.TemplateBindingExtension(Button.BackgroundProperty));
            borderFactory.SetValue(Border.PaddingProperty, new Thickness(10));

            var contentPresenterFactory = new System.Windows.FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(contentPresenterFactory);

            return borderFactory;
        }

        public void SearchStaff(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                StaffList = new ObservableCollection<Staff>(_allStaffList);
                return;
            }

            var filtered = _allStaffList.Where(s =>
                s.fullName.ToLower().Contains(keyword.ToLower()) ||
                s.StaffId.ToLower().Contains(keyword.ToLower()) ||
                s.phone.Contains(keyword) ||
                s.email.ToLower().Contains(keyword.ToLower())
            ).ToList();

            StaffList = new ObservableCollection<Staff>(filtered);
        }

        public void ThemNhanVien()
        {
            var addStaffWindow = new SaleManagerApp.Views.AddStaffWindow();
            addStaffWindow.ShowDialog();
            LoadStaffData();
        }

        public void EditStaff(string employeeId)
        {
            MessageBox.Show($"Chức năng sửa nhân viên {employeeId} đang được phát triển",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void DeleteStaff(string employeeId)
        {
            var staff = _allStaffList.FirstOrDefault(s => s.StaffId == employeeId);
            if (staff == null)
            {
                MessageBox.Show("Không tìm thấy nhân viên!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa nhân viên '{staff.fullName}' (Mã: {employeeId})?\n\n" +
                "Lưu ý: Dữ liệu chấm công và lương của nhân viên này cũng sẽ bị xóa!",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes)
                return;

            var deleteResult = _service.DeleteStaff(employeeId);

            if (deleteResult.Success)
            {
                MessageBox.Show(deleteResult.SuccessMessage, "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LoadStaffData();
            }
            else
            {
                MessageBox.Show(deleteResult.ErrorMessage, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}