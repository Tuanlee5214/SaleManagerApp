using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class ImportIngredientViewModel : BaseViewModel
    {
        private readonly WarehouseService _service = new WarehouseService();
        private readonly string _importOrderId = $"IM{DateTime.Now:yyyyMMddHHmmss}";

        // =========================
        // DATA SOURCE
        // =========================
        public ObservableCollection<IngredientItem> AllIngredients { get; }
            = new ObservableCollection<IngredientItem>();

        public ObservableCollection<IngredientItem> FilteredIngredients { get; }
            = new ObservableCollection<IngredientItem>();

        public Array IngredientGroups => Enum.GetValues(typeof(IngredientGroup));

        // =========================
        // FILTER
        // =========================
        private IngredientGroup _selectedGroup;
        public IngredientGroup SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        // =========================
        // SELECTED INGREDIENT
        // =========================
        private IngredientItem _selectedIngredient;
        public IngredientItem SelectedIngredient
        {
            get => _selectedIngredient;
            set
            {
                _selectedIngredient = value;
                OnPropertyChanged();
                LoadIngredientInfo();
            }
        }

        // =========================
        // DISPLAY INFO
        // =========================
        public string IngredientId => SelectedIngredient?.IngredientId;
        public string IngredientName => SelectedIngredient?.IngredientName;
        public string Unit => SelectedIngredient?.Unit;

        // =========================
        // INPUT
        // =========================
        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                _unitPrice = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _expiryDate;
        public DateTime? ExpiryDate
        {
            get => _expiryDate;
            set
            {
                _expiryDate = value;
                OnPropertyChanged();
            }
        }

        // =========================
        // COMMANDS
        // =========================
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        // =========================
        // ACTIONS
        // =========================
        public Action CloseAction { get; set; }
        public Action ReloadAction { get; set; }

        public ImportIngredientViewModel()
        {
            ConfirmCommand = new RelayCommand(Import);
            CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());
            LoadIngredients();
        }

        // =========================
        // LOAD DATA
        // =========================
        private void LoadIngredients()
        {
            var result = _service.GetAllIngredients();
            if (!result.Success)
            {
                ToastService.ShowError(result.ErrorMessage);
                return;
            }

            AllIngredients.Clear();
            foreach (var item in result.IngredientList)
                AllIngredients.Add(item);

            SelectedGroup = IngredientGroups.Cast<IngredientGroup>().First();
        }

        private void ApplyFilter()
        {
            FilteredIngredients.Clear();

            foreach (var item in AllIngredients.Where(i => i.Group == SelectedGroup))
                FilteredIngredients.Add(item);
        }

        private void LoadIngredientInfo()
        {
            OnPropertyChanged(nameof(IngredientId));
            OnPropertyChanged(nameof(IngredientName));
            OnPropertyChanged(nameof(Unit));
        }

        // =========================
        // IMPORT LOGIC
        // =========================
        private void Import(object obj)
        {
            if (SelectedIngredient == null)
            {
                ToastService.ShowError("Chưa chọn nguyên liệu");
                return;
            }

            if (Quantity <= 0)
            {
                ToastService.ShowError("Số lượng nhập phải lớn hơn 0");
                return;
            }

            if (UnitPrice <= 0)
            {
                ToastService.ShowError("Đơn giá không hợp lệ");
                return;
            }

            var result = _service.ImportIngredient(
                _importOrderId,
                SelectedIngredient.IngredientId,
                Quantity,
                UnitPrice,
                ExpiryDate
            );

            if (result.Success)
            {
                ReloadAction?.Invoke();
                ToastService.Show(result.SuccessMessage);
                CloseAction?.Invoke();
            }
            else
            {
                ToastService.ShowError(result.ErrorMessage);
            }
        }
    }
}
