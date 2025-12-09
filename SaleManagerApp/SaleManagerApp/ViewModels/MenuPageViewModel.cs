using SaleManagerApp.Navigation;
using SaleManagerApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class MenuPageViewModel:BaseViewModel
    {
        public ICommand OpenInsertMenuItemFormCommand { get; }
        public MenuPageViewModel()
        {
            OpenInsertMenuItemFormCommand = new RelayCommand(OpenInsertMenuItemForm);
        }

        public void OpenInsertMenuItemForm(object obj)
        {
            var vm = new InsertMenuItemViewModel();
            var window = new InsertMenuItemForm { DataContext = vm };
            vm.CloseAction = () => window.Close();
            window.ShowDialog();

        }
    }
}
