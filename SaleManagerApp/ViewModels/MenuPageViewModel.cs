using SaleManagerApp.Models;
using SaleManagerApp.Services;
using SaleManagerApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class MenuPageViewModel : BaseViewModel
    {
        private readonly MenuPageService _service = new MenuPageService();

        /* ===================== MENU ===================== */
        private string _selectedType;
        public string SelectedType
        {
            get => _selectedType;
            set { _selectedType = value; OnPropertyChanged(); }
        }
        public ObservableCollection<MenuItem> MenuItems { get; }
            = new ObservableCollection<MenuItem>();

        

        private string _type;
        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
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

        /* ===================== CART ===================== */

        public ObservableCollection<CartItem> CartItems { get; }
            = new ObservableCollection<CartItem>();

        public decimal TotalAmount => CartItems.Sum(x => x.SubTotal);

        private CartItem FindCartItem(MenuItem item)
        {
            return CartItems.FirstOrDefault(c => c.MenuItemId == item.menuItemId);
        }

        /* ===================== COMMANDS ===================== */

        public ICommand AddToCartCommand { get; }
        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand OpenInsertMenuItemFormCommand { get; }

        public ICommand OpenInsertCustomerFormCommand { get; }

        public ICommand OpenListTableWindowCommand { get; }

        public ICommand SelectAllCommand { get; }
        public ICommand SelectBeefCommand { get; }
        public ICommand SelectPorkCommand { get; }
        public ICommand SelectChickenCommand { get; }
        public ICommand SelectAnotherCommand { get; }
        public ICommand SelectDrinkCommand { get; }

        /* ===================== CONSTRUCTOR ===================== */

        public MenuPageViewModel()
        {
            SelectedType = "Tất cả";
            Type = null;
            OpenInsertMenuItemFormCommand = new RelayCommand(OpenInsertMenuItemForm);
            OpenInsertCustomerFormCommand = new RelayCommand(OpenInsertCustomerForm);
            OpenListTableWindowCommand = new RelayCommand(OpenListTableWindow);
            AddToCartCommand = new RelayCommand(o => AddToCart(o as MenuItem));
            IncreaseCommand = new RelayCommand(o => Increase(o as MenuItem));
            DecreaseCommand = new RelayCommand(o => Decrease(o as MenuItem));
            RemoveItemCommand = new RelayCommand(RemoveCartItem);

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

        /* ===================== CART LOGIC ===================== */

        private void AddToCart(MenuItem item)
        {
            if (item == null) return;

            var cartItem = FindCartItem(item);
            if (cartItem == null)
                CartItems.Add(new CartItem(item));
            else
                cartItem.Quantity++;

            SyncMenuDisplayQuantity();
            RaiseCartChanged();
        }

        private void Increase(MenuItem item)
        {
            if (item == null) return;

            var cartItem = FindCartItem(item);
            if (cartItem == null) return;

            cartItem.Quantity++;
            SyncMenuDisplayQuantity();
            RaiseCartChanged();
        }

        private void Decrease(MenuItem item)
        {
            if (item == null) return;

            var cartItem = FindCartItem(item);
            if (cartItem == null) return;

            if (cartItem.Quantity > 1)
                cartItem.Quantity--;
            else
                CartItems.Remove(cartItem);

            SyncMenuDisplayQuantity();
            RaiseCartChanged();
        }

        private void RemoveCartItem(object o)
        {
            if (o is  CartItem cartItem)
            {
                CartItems.Remove(cartItem);
                SyncMenuDisplayQuantity();
                RaiseCartChanged();
            } 
        }

        private void RaiseCartChanged()
        {
            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(TotalAmount));
        }

        /* ===================== SYNC ===================== */

        private void SyncMenuDisplayQuantity()
        {
            foreach (var menu in MenuItems)
            {
                var cartItem = CartItems
                    .FirstOrDefault(c => c.MenuItemId == menu.menuItemId);

                menu.DisplayQuantity = cartItem?.Quantity ?? 0;
            }
        }

        public void OpenInsertMenuItemForm(object obj)
        {
            var vm = new InsertMenuItemViewModel();
            vm.ReloadMenuItem = LoadMenuItems;
            var window = new InsertMenuItemForm { DataContext = vm };
            vm.CloseAction = () => window.Close();
            window.ShowDialog();
        }

        public void OpenListTableWindow(object obj)
        {
            var vm = new TableViewModel();
            var window = new ListTable { DataContext = vm };
            window.ShowDialog();
        }

        public void OpenInsertCustomerForm(object obj)
        {
            var vm = new InsertCustomerViewModel();
            var window = new InsertCustomerWindow { DataContext = vm };
            vm.CloseAction = () => window.Close();
            window.ShowDialog();
        }
        private void LoadMenuItems()
        {
            var result = _service.GetMenuItems(Type, SearchText);
            if (!result.Success) return;

            MenuItems.Clear();
            foreach (var item in result.MenuItemList)
                MenuItems.Add(item);

            SyncMenuDisplayQuantity();
        }

    }
}
