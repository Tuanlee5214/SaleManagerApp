using SaleManagerApp.Helpers;
using SaleManagerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class ImportWarehouseViewModel : BaseViewModel
    {
        private readonly ImportWarehouseService _service =
            new ImportWarehouseService();

        public string IngredientId { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }

        public ICommand ImportCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }
        public Action ReloadIngredient { get; set; }

        public ImportWarehouseViewModel()
        {
            ImportCommand = new RelayCommand(Import);
            CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());
        }

        private void Import(object obj)
        {
            if (Quantity <= 0)
            {
                ToastService.ShowError("Số lượng không hợp lệ");
                return;
            }

            var item = new ImportIngredientItem
            {
                ingredientId = IngredientId,
                quantity = Quantity,
                note = Note
            };

            var result = _service.ImportIngredient(item);

            if (result.Success)
            {
                ReloadIngredient?.Invoke();
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
