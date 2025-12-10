using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SaleManagerApp.ViewModels
{
    public class UserPageViewModel : BaseViewModel
    {
        private ObservableCollection<Staff> _allStaffList; // Lưu toàn bộ danh sách gốc
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
            // Khởi tạo danh sách nhân viên
            _allStaffList = new ObservableCollection<Staff>();
            StaffList = new ObservableCollection<Staff>();
            LoadStaffData();
        }

        private void LoadStaffData()
        {
            // Load dữ liệu nhân viên từ database hoặc service
            // Đây là dữ liệu mẫu
            _allStaffList.Clear();
            _allStaffList.Add(new Staff
            {
                Name = "Nguyễn Văn A",
                Code = "NV001",
                Birthday = "01/01/1990",
                StartDate = "01/01/2020",
                Phone = "0123456789",
                Email = "nguyenvana@example.com"
            });
            _allStaffList.Add(new Staff
            {
                Name = "Trần Thị B",
                Code = "NV002",
                Birthday = "15/05/1992",
                StartDate = "15/03/2021",
                Phone = "0987654321",
                Email = "tranthib@example.com"
            });

            // Copy sang StaffList để hiển thị
            StaffList = new ObservableCollection<Staff>(_allStaffList);
        }

        // Logic chấm công
        public void ChamCong()
        {
            DateTime now = DateTime.Now;
            TimeSpan currentTime = now.TimeOfDay;

            // Định nghĩa khung giờ chấm công hợp lệ: 19:00 - 20:00 (7PM - 8PM)
            TimeSpan startTime = new TimeSpan(19, 0, 0);  // 7:00 PM
            TimeSpan endTime = new TimeSpan(20, 0, 0);    // 8:00 PM

            bool isSuccess = false;

            // Kiểm tra nếu thời gian hiện tại trong khoảng 19:00 - 20:00
            if (currentTime >= startTime && currentTime < endTime)
            {
                isSuccess = true;
            }
            // Kiểm tra nếu qua 12h đêm (00:00 - 20:00 ngày hôm sau)
            else if (currentTime < endTime)
            {
                isSuccess = true;
            }

            // Hiển thị dialog thông báo
            ShowAttendanceDialog(isSuccess);

            // Lưu kết quả chấm công vào database
            if (isSuccess)
            {
                SaveAttendanceRecord(now);
            }
        }

        private void SaveAttendanceRecord(DateTime time)
        {
            // Logic lưu bản ghi chấm công vào database
            // TODO: Implement database logic
        }

        private void ShowAttendanceDialog(bool isSuccess)
        {
            // Tạo Window popup
            Window dialog = new Window
            {
                Width = 400,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                Background = System.Windows.Media.Brushes.Transparent,
                AllowsTransparency = true
            };

            // Tạo nội dung
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

            // Tiêu đề
            TextBlock title = new TextBlock
            {
                Text = isSuccess ? "CHẤM CÔNG THÀNH CÔNG" : "CHẤM CÔNG THẤT BẠI",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 30),
                Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(17, 24, 39))
            };

            // Button Quay lại
            Button backButton = new Button
            {
                Content = "Quay lại",
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

            // Bo tròn button
            backButton.Template = new System.Windows.Controls.ControlTemplate(typeof(Button))
            {
                VisualTree = CreateButtonTemplate()
            };

            backButton.Click += (s, e) => dialog.Close();

            // Thêm các control vào stack panel
            stackPanel.Children.Add(title);
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

        // Logic tìm kiếm nhân viên
        public void SearchStaff(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                // Hiển thị lại toàn bộ danh sách
                StaffList = new ObservableCollection<Staff>(_allStaffList);
                return;
            }

            // Filter danh sách theo keyword (case-insensitive)
            var filtered = _allStaffList.Where(s =>
                s.Name.ToLower().Contains(keyword.ToLower()) ||
                s.Code.ToLower().Contains(keyword.ToLower()) ||
                s.Phone.Contains(keyword) ||
                s.Email.ToLower().Contains(keyword.ToLower())
            ).ToList();

            StaffList = new ObservableCollection<Staff>(filtered);
        }

        // Logic thêm nhân viên
        public void ThemNhanVien()
        {
            var addStaffWindow = new SaleManagerApp.Views.AddStaffWindow();

            if (addStaffWindow.ShowDialog() == true && addStaffWindow.IsConfirmed)
            {
                // Tạo mã nhân viên tự động
                string newCode = $"NV{(_allStaffList.Count + 1):D3}";

                // Tạo đối tượng nhân viên mới
                var newStaff = new Staff
                {
                    Name = addStaffWindow.StaffName,
                    Code = newCode,
                    Birthday = addStaffWindow.Birthday?.ToString("dd/MM/yyyy"),
                    StartDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    Phone = addStaffWindow.Phone,
                    Email = addStaffWindow.Email
                };

                // Thêm vào danh sách gốc
                _allStaffList.Add(newStaff);

                // Thêm vào danh sách hiển thị
                StaffList.Add(newStaff);

                // Lưu vào database
                // TODO: Implement database save logic

                MessageBox.Show("Thêm nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    // Model cho Staff
    public class Staff
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Birthday { get; set; }
        public string StartDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}