using Microsoft.Win32;
using SaleManagerApp.Helpers;
using SaleManagerApp.Model;
using SaleManagerApp.Services;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class AddStaffViewModel : BaseViewModel
    {
        private readonly StaffManagementService _service = new StaffManagementService();

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private DateTime? _dateOfBirth;
        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set { _dateOfBirth = value; OnPropertyChanged(); }
        }

        private string _position;
        public string Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(); }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _previewPath;
        public string PreviewPath
        {
            get => _previewPath;
            set
            {
                _previewPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasImage));
            }
        }

        public bool HasImage => !string.IsNullOrEmpty(PreviewPath);

        private string _imageUrl;
        public string ImageUrl
        {
            get => _imageUrl;
            set { _imageUrl = value; OnPropertyChanged(); }
        }

        public ICommand PickImageCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }

        public AddStaffViewModel()
        {
            PickImageCommand = new RelayCommand(SelectImage);
            ConfirmCommand = new RelayCommand(SaveStaff);
            CancelCommand = new RelayCommand(CancelForm);
        }

        public void CancelForm(object obj)
        {
            CloseAction?.Invoke();
        }

        // ĐỔI TÊN TỪ PickImage THÀNH SelectImage
        public void SelectImage(object obj)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
            dialog.Title = "Chọn ảnh đại diện";

            if (dialog.ShowDialog() != true)
                return;

            string originalPath = dialog.FileName;

            if (!File.Exists(originalPath))
            {
                MessageBox.Show("File không tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Set preview path để hiển thị ảnh ngay lập tức
                PreviewPath = originalPath;

                // Tạo thư mục đích
                string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                string targetFolder = Path.Combine(appFolder, "Images", "Staffs");

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                // Tạo tên file mới
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(originalPath);
                string targetPath = Path.Combine(targetFolder, fileName);

                // Copy file
                File.Copy(originalPath, targetPath, true);

                // Lưu đường dẫn tương đối vào DB
                ImageUrl = $"Images/Staffs/{fileName}";

                // DEBUG INFO
                System.Diagnostics.Debug.WriteLine("===== IMAGE SAVE INFO =====");
                System.Diagnostics.Debug.WriteLine($"Original: {originalPath}");
                System.Diagnostics.Debug.WriteLine($"Target: {targetPath}");
                System.Diagnostics.Debug.WriteLine($"Relative path for DB: {ImageUrl}");
                System.Diagnostics.Debug.WriteLine($"File copied successfully: {File.Exists(targetPath)}");
                System.Diagnostics.Debug.WriteLine("===========================");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu ảnh: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                PreviewPath = null;
                ImageUrl = null;
            }
        }

        // ĐỔI TÊN TỪ InsertStaff THÀNH SaveStaff
        public void SaveStaff(object obj)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(FullName))
            {
                MessageBox.Show("Vui lòng nhập họ và tên!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DateOfBirth == null)
            {
                MessageBox.Show("Vui lòng chọn ngày sinh!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra tuổi hợp lệ (>= 18 tuổi)
            var age = DateTime.Now.Year - DateOfBirth.Value.Year;
            if (age < 18)
            {
                MessageBox.Show("Nhân viên phải từ 18 tuổi trở lên!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Position))
            {
                MessageBox.Show("Vui lòng nhập chức vụ!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate phone format
            if (!System.Text.RegularExpressions.Regex.IsMatch(Phone, @"^0\d{9,10}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ! (VD: 0901234567)", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate email format
            if (!System.Text.RegularExpressions.Regex.IsMatch(Email,
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                MessageBox.Show("Email không hợp lệ!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // DEBUG: In ra thông tin ảnh trước khi lưu vào DB
            System.Diagnostics.Debug.WriteLine("===== BEFORE INSERT TO DB =====");
            System.Diagnostics.Debug.WriteLine($"PreviewPath: {PreviewPath}");
            System.Diagnostics.Debug.WriteLine($"ImageUrl (will save to DB): {ImageUrl}");
            if (!string.IsNullOrEmpty(ImageUrl))
            {
                string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                string checkPath = Path.Combine(appFolder, ImageUrl.Replace("/", "\\"));
                System.Diagnostics.Debug.WriteLine($"Full path check: {checkPath}");
                System.Diagnostics.Debug.WriteLine($"File exists: {File.Exists(checkPath)}");
            }
            System.Diagnostics.Debug.WriteLine("===============================");

            // TẠO STAFF VỚI ĐẦY ĐỦ 6 FIELDS
            Staff staff = new Staff
            {
                fullName = this.FullName.Trim(),
                dateofBirth = this.DateOfBirth.Value.ToString("yyyy-MM-dd"),
                position = this.Position.Trim(),
                phone = this.Phone.Trim(),
                email = this.Email.Trim(),
                ImagePath = this.ImageUrl // Có thể null
            };

            var result = _service.InsertStaffÌnormation(staff);

            if (result.Success)
            {
                MessageBox.Show(result.SuccessMessage, "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                CloseAction?.Invoke();
            }
            else
            {
                MessageBox.Show(result.ErrorMessage, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}