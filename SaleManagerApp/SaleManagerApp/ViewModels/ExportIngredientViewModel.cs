using SaleManagerApp.Helpers;
using SaleManagerApp.Services;
using System;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class ExportIngredientViewModel : BaseViewModel
    {
        private readonly WarehouseService _service = new WarehouseService();

        public string IngredientId { get; set; }
        public int CurrentQuantity { get; set; }

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

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }
        public Action ReloadAction { get; set; }

        public ExportIngredientViewModel()
        {
            ConfirmCommand = new RelayCommand(Export);
            CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());
        }

        private void Export(object obj)
        {
            if (Quantity <= 0)
            {
                ToastService.ShowError("Số lượng xuất không hợp lệ");
                return;
            }

            if (Quantity > CurrentQuantity)
            {
                ToastService.ShowError("Số lượng không đủ để xuất kho");
                return;
            }

            var result = _service.ExportIngredient(IngredientId, Quantity);
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
