using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SaleManagerApp.ViewModels
{
    public class CreateIngredientViewModel : BaseViewModel
    {
        // ❗ FIX C# 7.3
        private readonly WarehouseService _service = new WarehouseService();

        // =========================
        // INPUT
        // =========================
        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _unit;
        public string Unit
        {
            get => _unit;
            set { _unit = value; OnPropertyChanged(); }
        }

        private string _filter;
        public string Filter
        {
            get => _filter;
            set { _filter = value; OnPropertyChanged(); }
        }

        private int _minQuantity;
        public int MinQuantity
        {
            get => _minQuantity;
            set { _minQuantity = value; OnPropertyChanged(); }
        }

        private int _maxStorageDays = 7;
        public int MaxStorageDays
        {
            get => _maxStorageDays;
            set { _maxStorageDays = value; OnPropertyChanged(); }
        }

        // =========================
        // IMAGE
        // =========================
        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            private set { _imagePath = value; OnPropertyChanged(); }
        }

        private ImageSource _imagePreview;
        public ImageSource ImagePreview
        {
            get => _imagePreview;
            private set { _imagePreview = value; OnPropertyChanged(); }
        }

        // =========================
        // STATIC - DANH SÁCH FILTER
        // =========================
        public List<string> Filters { get; } = new List<string>
        {
            "Meat",
            "Seafood",
            "Vegetable",
            "Spice",
            "Others"
        };

        // =========================
        // COMMANDS
        // =========================
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectImageCommand { get; }

        // =========================
        // ACTIONS
        // =========================
        public Action CloseAction { get; set; }
        public Action ReloadAction { get; set; }

        public CreateIngredientViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());
            SelectImageCommand = new RelayCommand(_ => SelectImage());

            Filter = "Others";
        }

        // =========================
        // IMAGE PICKER
        // =========================
        private void SelectImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.png)|*.jpg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                ImagePath = dialog.FileName;
                ImagePreview = new BitmapImage(new Uri(dialog.FileName));
            }
        }

        // =========================
        // SAVE
        // =========================
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ToastService.ShowError("Tên nguyên liệu không hợp lệ");
                return;
            }

            if (string.IsNullOrWhiteSpace(Unit))
            {
                ToastService.ShowError("Đơn vị không hợp lệ");
                return;
            }

            if (MinQuantity < 0)
            {
                ToastService.ShowError("Ngưỡng cảnh báo không hợp lệ");
                return;
            }

            if (string.IsNullOrWhiteSpace(Filter))
            {
                ToastService.ShowError("Chưa chọn nhóm nguyên liệu");
                return;
            }

            if (MaxStorageDays <= 0)
            {
                ToastService.ShowError("Số ngày lưu trữ phải > 0");
                return;
            }

            try
            {
                var ingredient = new Ingredient
                {
                    IngredientName = Name,
                    Unit = Unit,
                    Filter = Filter,
                    MinQuantity = MinQuantity,
                    MaxStorageDays = MaxStorageDays,
                    ImageUrl = ImagePath,
                    CreatedAt = DateTime.Now
                };

                _service.CreateIngredient(ingredient);

                ReloadAction?.Invoke();
                ToastService.Show("Thêm nguyên liệu thành công");
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Không thể thêm nguyên liệu: {ex.Message}");
            }
        }
    }
}