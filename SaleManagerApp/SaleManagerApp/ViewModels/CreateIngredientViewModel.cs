using SaleManagerApp.Helpers;
using SaleManagerApp.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace SaleManagerApp.ViewModels
{
    public class CreateIngredientViewModel : BaseViewModel
    {
        private readonly WarehouseService _service = new WarehouseService();

        // =========================
        // INPUT FIELDS
        // =========================

        private string _ingredientName;
        public string IngredientName
        {
            get => _ingredientName;
            set
            {
                _ingredientName = value;
                OnPropertyChanged();
            }
        }

        private string _unit;
        public string Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                OnPropertyChanged();
            }
        }

        private string _selectedGroup;
        public string SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                OnPropertyChanged();
            }
        }

        private int _minQuantity;
        public int MinQuantity
        {
            get => _minQuantity;
            set
            {
                _minQuantity = value;
                OnPropertyChanged();
            }
        }

        // =========================
        // IMAGE
        // =========================

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            private set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _imagePreview;
        public ImageSource ImagePreview
        {
            get => _imagePreview;
            private set
            {
                _imagePreview = value;
                OnPropertyChanged();
            }
        }

        // =========================
        // STATIC DATA
        // =========================

        public ObservableCollection<string> Groups { get; }
            = new ObservableCollection<string>
            {
                "Thịt",
                "Hải sản",
                "Rau củ",
                "Gia vị",
                "Pha chế"
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

                ImagePreview = new BitmapImage(
                    new Uri(dialog.FileName, UriKind.Absolute)
                );
            }
        }

        // =========================
        // SAVE LOGIC
        // =========================

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(IngredientName))
            {
                ToastService.ShowError("Tên nguyên liệu không được để trống");
                return;
            }

            if (string.IsNullOrWhiteSpace(Unit))
            {
                ToastService.ShowError("Đơn vị không được để trống");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedGroup))
            {
                ToastService.ShowError("Chưa chọn nhóm nguyên liệu");
                return;
            }

            if (MinQuantity < 0)
            {
                ToastService.ShowError("Ngưỡng cảnh báo không hợp lệ");
                return;
            }

            var result = _service.CreateIngredient(
                IngredientName,
                Unit,
                SelectedGroup,
                MinQuantity,
                ImagePath
            );

            if (result.Success)
            {
                ToastService.Show(result.SuccessMessage);
                ReloadAction?.Invoke();
                CloseAction?.Invoke();
            }
            else
            {
                ToastService.ShowError(result.ErrorMessage);
            }
        }
    }
}
