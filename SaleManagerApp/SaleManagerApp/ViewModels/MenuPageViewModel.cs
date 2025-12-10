using SaleManagerApp.Models;
using SaleManagerApp.Navigation;
using SaleManagerApp.Services;
using SaleManagerApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class MenuPageViewModel:BaseViewModel
    {
        public MenuPageService _service = new MenuPageService();
        private string _selectedType;
        public string SelectedType
        {
            get => _selectedType;
            set { _selectedType = value; OnPropertyChanged();}
        }

        private string _type;
        public string Type
        {
            get => _type;
            set
            {
                _type = value; OnPropertyChanged();
                LoadMenuItems();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadMenuItems();
            }
        }

        public ObservableCollection<MenuItem> MenuItems { get; } = new ObservableCollection<MenuItem>();


        public ICommand OpenInsertMenuItemFormCommand { get; }
        public ICommand SelectAllCommand { get; }
        public ICommand SelectBeefCommand { get; }
        public ICommand SelectPorkCommand { get; }
        public ICommand SelectChickenCommand { get; }
        public ICommand SelectAnotherCommand { get; }
        public ICommand SelectDrinkCommand { get; }
        public MenuPageViewModel()
        {
            SelectedType = "Tất cả";
            Type = null;
            OpenInsertMenuItemFormCommand = new RelayCommand(OpenInsertMenuItemForm);
            SelectAllCommand = new RelayCommand(o =>
            {
                SelectedType = "Tất cả";
                Type = null;
                Console.WriteLine("Selected = " + SelectedType);
            });

            SelectBeefCommand = new RelayCommand(o =>
            {
                SelectedType = "Thịt bò";
                Type = "Thịt bò";
                Console.WriteLine("Selected = " + SelectedType);
            });

            SelectPorkCommand = new RelayCommand(o =>
            {
                SelectedType = "Thịt heo";
                Type = "Thịt heo";
                Console.WriteLine("Selected = " + SelectedType);
            });

            SelectChickenCommand = new RelayCommand(o =>
            {
                SelectedType = "Thịt gà";
                Type = "Thịt gà";
                Console.WriteLine("Selected = " + SelectedType);
            });

            SelectAnotherCommand = new RelayCommand(o =>
            {
                SelectedType = "Món khác";
                Type = "Món khác";
                Console.WriteLine("Selected = " + SelectedType);
            });

            SelectDrinkCommand = new RelayCommand(o =>
            {
                SelectedType = "Nước uống";
                Type = "Nước uống";
                Console.WriteLine("Selected = " + SelectedType);
            });
            LoadMenuItems();
        }

        public void OpenInsertMenuItemForm(object obj)
        {
            var vm = new InsertMenuItemViewModel();
            vm.ReloadMenuItem = LoadMenuItems;
            var window = new InsertMenuItemForm { DataContext = vm };
            vm.CloseAction = () => window.Close();
            window.ShowDialog();
        }

        private void LoadMenuItems()
        {
            var type = string.IsNullOrWhiteSpace(Type) ? null : Type;
            var search = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText;

            var result = _service.GetMenuItems(type, search);
            if(result.Success)
            {
                MenuItems.Clear();
                foreach (var item in result.MenuItemList)
                MenuItems.Add(item);
            }
        }
    }
}
