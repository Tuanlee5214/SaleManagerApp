using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class ExportIngredientViewModel : BaseViewModel
    {
        private readonly WarehouseService _service = new WarehouseService();
        private readonly string _exportOrderId = $"EX{DateTime.Now:yyyyMMddHHmmss}";

        // =========================
        // INGREDIENT
        // =========================
        private IngredientItem _ingredient;
        public IngredientItem Ingredient
        {
            get => _ingredient;
            private set
            {
                _ingredient = value;
                OnPropertyChanged();
            }
        }

        public string IngredientId => Ingredient?.IngredientId;
        public int CurrentQuantity => Ingredient?.TotalQuantity ?? 0;
        public bool HasExpiredBatch => Ingredient?.HasExpiredBatch ?? false;

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

        public ExportIngredientViewModel()
        {
            ConfirmCommand = new RelayCommand(Export);
            CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());
        }

        // =========================
        // SET INGREDIENT
        // =========================
        public void SetIngredient(IngredientItem item)
        {
            Ingredient = item;
            OnPropertyChanged(nameof(IngredientId));
            OnPropertyChanged(nameof(CurrentQuantity));
            OnPropertyChanged(nameof(HasExpiredBatch));
        }

        // =========================
        // EXPORT LOGIC
        // =========================
        private void Export(object obj)
        {
            if (Ingredient == null)
            {
                ToastService.ShowError("Chưa chọn nguyên liệu");
                return;
            }

            if (HasExpiredBatch)
            {
                ToastService.ShowError("Nguyên liệu có batch đã hết hạn, cần xử lý trước");
                return;
            }

            if (Quantity <= 0)
            {
                ToastService.ShowError("Số lượng xuất phải lớn hơn 0");
                return;
            }

            if (Quantity > CurrentQuantity)
            {
                ToastService.ShowError("Số lượng tồn kho không đủ");
                return;
            }

            var result = _service.ExportIngredient(
                _exportOrderId,
                Ingredient.IngredientId,
                Quantity
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
