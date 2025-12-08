using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using SaleManagerApp.Helpers;
using System.IO;
using Microsoft.Win32;


namespace SaleManagerApp.ViewModels
{
    public class InsertMenuItemViewModel : BaseViewModel
    {

        private readonly MenuPageService _service = new MenuPageService();
        public List<string> Catagories { get; set; }
        //Tham chiếu tới ô trong form nhập là các property này
        private string _menuItemName;
        public string MenuItemName
        {
            get => _menuItemName;
            set { _menuItemName = value; OnPropertyChanged();}
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get => _unitPrice;
            set{_unitPrice = value; OnPropertyChanged();}
        }

        private string _imageUrl;
        public string ImageUrl
        {
            get => _imageUrl;
            set{_imageUrl = value; OnPropertyChanged();}
        }

        private string _size;
        public string Size
        {
            get => _size;
            set{_size = value; OnPropertyChanged();}
        }

        private string _specialInfo;
        public string SpecialInfo
        {
            get => _specialInfo;
            set{_specialInfo = value; OnPropertyChanged();}
        }

        private string _type;
        public string Type
        {
            get => _type;
            set{_type = value;OnPropertyChanged();}
        }

        //Biến thông báo lỗi
        private string _errorMessage;
        private string _successMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set { _successMessage = value; OnPropertyChanged(); }
        }

        //Hiển thị ảnh tạm thời
        private string _previewPath;   
        public string PreviewPath
        {
            get => _previewPath;
            set { _previewPath = value; OnPropertyChanged(); }
        }

        //Khai báo command để gọi hàm thực thi sau khi bấm nút xác nhận thêm menuitem
        public ICommand InsertMenuItemCommand { get; }
        public ICommand PickImageCommand { get; }

        public ICommand CancelFormCommand { get; }

        public InsertMenuItemViewModel()
        {
            InsertMenuItemCommand = new RelayCommand(InsertMenuItem);
            PickImageCommand = new RelayCommand(PickImage);
            CancelFormCommand = new RelayCommand(CancelForm);
            Catagories = new List<string>
            {
                "Nước uống",
                "Thịt heo",
                "Thịt bò",
                "Thịt gà"
            };
        }

        public Action CloseAction { get; set; }

        public void CancelForm(object obj)
        {
            CloseAction?.Invoke();
        }
        public void PickImage(object obj)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";

            if (dialog.ShowDialog() != true)
                return;

            string originalPath = dialog.FileName;

            // Preview ngay cho người dùng
            PreviewPath = originalPath;

            // Copy vào thư mục của app
            string appFolder = AppDomain.CurrentDomain.BaseDirectory;
            string targetFolder = Path.Combine(appFolder, "Images", "MenuItems");

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            // Đặt tên theo GUID để tránh trùng
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(originalPath);
            string targetPath = Path.Combine(targetFolder, fileName);

            File.Copy(originalPath, targetPath, true);

            // Lưu đường dẫn tương đối để lưu vào DB
            this.ImageUrl = $"Images/MenuItems/{fileName}";
        }

        public void InsertMenuItem(Object obj)
        {
            MenuItem item = new MenuItem();
            item.menuItemName = this.MenuItemName;
            item.unitPrice = this.UnitPrice;
            item.imageUrl = this.ImageUrl;
            item.size = this.Size;
            item.specialInfo = this.SpecialInfo;
            item.type = this.Type;
            var result = _service.InsertMenuItem(item);
            if(result.Success)
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ToastService.Show(result.SuccessMessage);
                    CloseAction?.Invoke();
                }, System.Windows.Threading.DispatcherPriority.Loaded);
            }
            else
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ToastService.ShowError(result.ErrorMessage);
                    Console.WriteLine(result.ErrorMessage);
                    CloseAction?.Invoke();
                }, System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }
    }
}
