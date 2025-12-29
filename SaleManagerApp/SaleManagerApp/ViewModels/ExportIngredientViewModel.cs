using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System;
using System.Linq;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class ExportIngredientViewModel : BaseViewModel
    {
        // ❌ new()  →  ✅ new WarehouseService()
        private readonly WarehouseService _service = new WarehouseService();
        private readonly string _exportOrderId = $"EX{DateTime.Now:yyyyMMddHHmmss}";

        // =========================
        // DATA
        // =========================
        public string IngredientId { get; set; }

        private int _currentQuantity;
        public int CurrentQuantity
        {
            get => _currentQuantity;
            set { _currentQuantity = value; OnPropertyChanged(); }
        }

        // =========================
        // INPUT
        // =========================
        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
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
            ConfirmCommand = new RelayCommand(_ => Export());
            CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());
        }

        // =========================
        // EXPORT
        // =========================
        private void Export()
        {
            if (Quantity <= 0)
            {
                ToastService.ShowError("Số lượng xuất không hợp lệ");
                return;
            }

            if (Quantity > CurrentQuantity)
            {
                ToastService.ShowError("Không đủ tồn kho");
                return;
            }

            try
            {
                _service.ExportIngredient(
                    IngredientId,
                    Quantity,
                    "EMP001",          // TODO: lấy từ user đăng nhập
                    "Xuất kho"
                );

                ReloadAction?.Invoke();
                ToastService.Show("Xuất kho thành công");
                CloseAction?.Invoke();
            }
            catch
            {
                ToastService.ShowError("Xuất kho không thành công");
            }
        }
    }
}
