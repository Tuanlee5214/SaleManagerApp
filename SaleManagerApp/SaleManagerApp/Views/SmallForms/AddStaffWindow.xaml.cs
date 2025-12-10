using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SaleManagerApp.Views
{
    public partial class AddStaffWindow : Window
    {
        private string _selectedImagePath;

        public string StaffName { get; private set; }
        public DateTime? Birthday { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public string ImagePath { get; private set; }
        public bool IsConfirmed { get; private set; }

        public AddStaffWindow()
        {
            InitializeComponent();
            IsConfirmed = false;
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                Title = "Chọn ảnh đại diện"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;

                // Hiển thị preview ảnh
                var template = UploadImageButton.Template;
                var previewImage = template.FindName("PreviewImage", UploadImageButton) as Image;

                if (previewImage != null)
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(_selectedImagePath);
                    bitmap.DecodePixelWidth = 80;
                    bitmap.EndInit();

                    previewImage.Source = bitmap;
                    previewImage.Visibility = Visibility.Visible;
                }
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (BirthdayDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày sinh!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lưu dữ liệu
            StaffName = NameTextBox.Text;
            Birthday = BirthdayDatePicker.SelectedDate;
            Phone = PhoneTextBox.Text;
            Email = EmailTextBox.Text;
            ImagePath = _selectedImagePath;
            IsConfirmed = true;

            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.DialogResult = false;
            this.Close();
        }
    }
}