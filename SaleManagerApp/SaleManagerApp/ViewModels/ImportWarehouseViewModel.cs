using SaleManagerApp.Helpers;
using SaleManagerApp.Services;
using System;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class ImportIngredientViewModel : BaseViewModel
    {
        private readonly WarehouseService _service = new WarehouseService();

        public string IngredientId { get; set; }

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

        public ImportIngredientViewModel()
        {
            ConfirmCommand = new RelayCommand(Import);
            CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());
        }

        private void Import(object obj)
        {
            if (Quantity <= 0)
            {
                ToastService.ShowError("Số lượng nhập không hợp lệ");
                return;
            }

            var result = _service.Import(IngredientId, Quantity);
            if (result.Success)
            {
                ReloadAction?.Invoke();
                ToastService.Show(result.Message);
                CloseAction?.Invoke();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }
    }
}
