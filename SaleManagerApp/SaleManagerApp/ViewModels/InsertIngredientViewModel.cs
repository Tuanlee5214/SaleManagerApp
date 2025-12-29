using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class ImportIngredientViewModel : BaseViewModel
    {
        // ❌ new() → ✅ new WarehouseService()
        private readonly WarehouseService _service = new WarehouseService();

        // =========================
        // DATA
        // =========================
        // ❌ new() → ✅ new ObservableCollection<IngredientItem>()
        public ObservableCollection<IngredientItem> AllIngredients { get; }
            = new ObservableCollection<IngredientItem>();

        // ❌ new() → ✅ new ObservableCollection<IngredientItem>()
        public ObservableCollection<IngredientItem> FilteredIngredients { get; }
            = new ObservableCollection<IngredientItem>();

        // Danh sách filter
        public List<string> Filters { get; } = new List<string>
        {
            "Meat",
            "Seafood",
            "Vegetable",
            "Spice",
            "Others"
        };

        // =========================
        // FILTER
        // =========================
        private string _selectedFilter;
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
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
                OnPropertyChanged(nameof(IngredientId));
                OnPropertyChanged(nameof(IngredientName));
                OnPropertyChanged(nameof(Unit));
                OnPropertyChanged(nameof(MaxStorageDays));
            }
        }

        public string IngredientId => SelectedIngredient?.IngredientId;
        public string IngredientName => SelectedIngredient?.IngredientName;
        public string Unit => SelectedIngredient?.Unit;
        public int MaxStorageDays => SelectedIngredient?.MaxStorageDays ?? 7;

        // =========================
        // INPUT
        // =========================
        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        private DateTime _importDate = DateTime.Today;
        public DateTime ImportDate
        {
            get => _importDate;
            set { _importDate = value; OnPropertyChanged(); UpdateExpiryDate(); }
        }

        private DateTime _expiryDate;
        public DateTime ExpiryDate
        {
            get => _expiryDate;
            set { _expiryDate = value; OnPropertyChanged(); }
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
            ConfirmCommand = new RelayCommand(_ => Import());
            CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());

            LoadIngredients();
        }

        // =========================
        // LOAD
        // =========================
        private void LoadIngredients()
        {
            try
            {
                var data = _service.GetAllIngredients();

                AllIngredients.Clear();
                foreach (var item in data)
                    AllIngredients.Add(item);

                if (Filters.Any())
                {
                    SelectedFilter = Filters.First();
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Không thể tải danh sách nguyên liệu: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            FilteredIngredients.Clear();

            if (string.IsNullOrEmpty(SelectedFilter))
            {
                foreach (var item in AllIngredients)
                    FilteredIngredients.Add(item);
            }
            else
            {
                foreach (var item in AllIngredients.Where(i => i.Filter == SelectedFilter))
                    FilteredIngredients.Add(item);
            }
        }

        // =========================
        // AUTO UPDATE EXPIRY DATE
        // =========================
        private void UpdateExpiryDate()
        {
            if (SelectedIngredient != null)
            {
                ExpiryDate = ImportDate.AddDays(SelectedIngredient.MaxStorageDays);
            }
        }

        // =========================
        // IMPORT
        // =========================
        private void Import()
        {
            if (SelectedIngredient == null)
            {
                ToastService.ShowError("Chưa chọn nguyên liệu");
                return;
            }

            if (Quantity <= 0)
            {
                ToastService.ShowError("Số lượng phải > 0");
                return;
            }

            if (ExpiryDate <= ImportDate)
            {
                ToastService.ShowError("Ngày hết hạn phải sau ngày nhập");
                return;
            }

            try
            {
                _service.ImportIngredient(
                    SelectedIngredient.IngredientId,
                    Quantity,
                    ImportDate,
                    ExpiryDate,
                    $"Nhập {Quantity} {Unit}"
                );

                ReloadAction?.Invoke();
                ToastService.Show("Nhập kho thành công");
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Nhập kho thất bại: {ex.Message}");
            }
        }
    }
}
