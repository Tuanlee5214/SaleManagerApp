using SaleManagerApp.Models;
using SaleManagerApp.Navigation;
using SaleManagerApp.Services;
using SaleManagerApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

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

        //public IEnumerable<MenuItem> CartItems
        //    => MenuItems.Where(x => x.Quantity > 0);


        //public decimal TotalAmount =>
        //    MenuItems
        //        .Where(x => x.Quantity > 0)
        //        .Sum(x => x.Quantity * x.unitPrice);

        //private void MenuItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(MenuItem.Quantity))
        //    {
        //        OnPropertyChanged(nameof(CartItems));
        //        OnPropertyChanged(nameof(TotalAmount));
        //    }
        //}

        public ObservableCollection<MenuItem> CartItems { get; }
            = new ObservableCollection<MenuItem>();

        public decimal TotalAmount =>
            CartItems.Sum(x => x.Quantity * x.unitPrice);

        private void MenuItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MenuItem.Quantity))
            {
                var item = (MenuItem)sender;

                if (item.Quantity > 0 && !CartItems.Contains(item))
                    CartItems.Add(item);

                if (item.Quantity == 0 && CartItems.Contains(item))
                    CartItems.Remove(item);

                OnPropertyChanged(nameof(TotalAmount));
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
        public ICommand OpenInsertCustomerFormCommand { get; }
        public ICommand RemoveItemCommand { get; }


        public MenuPageViewModel()
        {
            SelectedType = "Tất cả";
            Type = null;
            OpenInsertMenuItemFormCommand = new RelayCommand(OpenInsertMenuItemForm);
            OpenInsertCustomerFormCommand = new RelayCommand(OpenInsertCustomerForm);
            //RemoveItemCommand = new RelayCommand(o =>
            //{
            //    if (o is MenuItem item)
            //        RemoveItem(item);
            //});

            RemoveItemCommand = new RelayCommand(RemoveItem);
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

        //public void RemoveItem(MenuItem item)
        //{
        //    if (item == null) return;
        //    item.Quantity = 0;
        //    OnPropertyChanged(nameof(CartItems));
        //    OnPropertyChanged(nameof(TotalAmount));
        //}

        public void RemoveItem(object obj)
        {
            if (obj is MenuItem item)
                item.Quantity = 0;   
        }
        public void OpenInsertCustomerForm(object obj)
        {
            var vm = new InsertCustomerViewModel();
            vm.ReloadCustomer = LoadMenuItems;
            var window = new InsertCustomerWindow { DataContext = vm };
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
                {
                    item.PropertyChanged += MenuItem_PropertyChanged;
                    MenuItems.Add(item);
                }

                //OnPropertyChanged(nameof(CartItems));
                //OnPropertyChanged(nameof(TotalAmount));
            }
        }
    }
}
