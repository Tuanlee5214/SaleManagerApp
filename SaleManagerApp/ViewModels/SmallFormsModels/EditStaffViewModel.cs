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
    public class EditStaffViewModel : BaseViewModel
    {
        private readonly StaffManagementService _service = new StaffManagementService();
        private readonly Staff _originalStaff;

        private string _staffId;
        public string StaffId
        {
            get => _staffId;
            set { _staffId = value; OnPropertyChanged(); }
        }

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

        private bool _isImageChanged = false;

        public ICommand PickImageCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }
        public Action SaveSuccessAction { get; set; }

        public EditStaffViewModel(Staff staff)
        {
            _originalStaff = staff;

            StaffId = staff.StaffId;
            FullName = staff.fullName;

            if (DateTime.TryParseExact(staff.dateofBirth, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime dob))
            {
                DateOfBirth = dob;
            }

            Position = staff.position;
            Phone = staff.phone;
            Email = staff.email;

            // Hiển thị ảnh hiện tại (nếu có)
            if (!string.IsNullOrEmpty(staff.ImagePath))
            {
                PreviewPath = staff.ImagePath; // Absolute path để hiển thị

                // Lấy relative path từ absolute path để lưu vào DB
                try
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    if (staff.ImagePath.StartsWith(basePath))
                    {
                        ImageUrl = staff.ImagePath.Substring(basePath.Length).Replace("\\", "/");
                    }
                    else
                    {
                        ImageUrl = staff.ImagePath;
                    }
                }
                catch
                {
                    ImageUrl = staff.ImagePath;
                }
            }

            PickImageCommand = new RelayCommand(SelectImage);
            SaveCommand = new RelayCommand(SaveChanges);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SelectImage(object obj)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp",
                Title = "Chọn ảnh đại diện"
            };

            if (dialog.ShowDialog() != true)
                return;

            string originalPath = dialog.FileName;

            if (!File.Exists(originalPath))
            {
                ToastService.ShowError("File không tồn tại!");
                return;
            }

            try
            {
                // Hiển thị preview ngay lập tức
                PreviewPath = originalPath;

                // Copy file vào thư mục Images/Staffs
                string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                string targetFolder = Path.Combine(appFolder, "Images", "Staffs");

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(originalPath);
                string targetPath = Path.Combine(targetFolder, fileName);

                File.Copy(originalPath, targetPath, true);

                // LƯU RELATIVE PATH (ngắn gọn) thay vì absolute path
                ImageUrl = $"Images/Staffs/{fileName}";
                _isImageChanged = true;

                ToastService.Show("Đã chọn ảnh mới!");
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Lỗi khi lưu ảnh: {ex.Message}");
                PreviewPath = _originalStaff.ImagePath; // Khôi phục ảnh cũ
                _isImageChanged = false;
            }
        }

        private void SaveChanges(object obj)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ToastService.ShowError("Vui lòng nhập họ và tên!");
                return;
            }

            if (DateOfBirth == null)
            {
                ToastService.ShowError("Vui lòng chọn ngày sinh!");
                return;
            }

            var age = DateTime.Now.Year - DateOfBirth.Value.Year;
            if (age < 18)
            {
                ToastService.ShowError("Nhân viên phải từ 18 tuổi trở lên!");
                return;
            }

            if (string.IsNullOrWhiteSpace(Position))
            {
                ToastService.ShowError("Vui lòng nhập chức vụ!");
                return;
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                ToastService.ShowError("Vui lòng nhập số điện thoại!");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(Phone, @"^0\d{9,10}$"))
            {
                ToastService.ShowError("Số điện thoại không hợp lệ! (VD: 0901234567)");
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ToastService.ShowError("Vui lòng nhập email!");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(Email,
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                ToastService.ShowError("Email không hợp lệ!");
                return;
            }

            // Kiểm tra độ dài ImageUrl
            if (!string.IsNullOrEmpty(ImageUrl) && ImageUrl.Length > 200)
            {
                ToastService.ShowError($"Đường dẫn ảnh quá dài ({ImageUrl.Length} ký tự). Vui lòng chọn ảnh khác!");
                return;
            }

            Staff updatedStaff = new Staff
            {
                StaffId = this.StaffId,
                fullName = this.FullName.Trim(),
                dateofBirth = this.DateOfBirth.Value.ToString("yyyy-MM-dd"),
                position = this.Position.Trim(),
                phone = this.Phone.Trim(),
                email = this.Email.Trim(),
                ImagePath = this.ImageUrl // Lưu RELATIVE PATH
            };

            var result = _service.UpdateEmployee(updatedStaff);

            if (result.Success)
            {
                ToastService.Show(result.Message);

                // GỌI SaveSuccessAction TRƯỚC (set DialogResult = true)
                // Action này sẽ tự động đóng window
                SaveSuccessAction?.Invoke();  // ✅ ĐÚNG - Chỉ gọi cái này thôi
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        private void Cancel(object obj)
        {
            CloseAction?.Invoke();
        }
    }
}