using SaleManagerApp.Models;
using SaleManagerApp.Navigation;
using SaleManagerApp.Services;
using SaleManagerApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace SaleManagerApp.ViewModels
{
    public class MenuPageViewModel : BaseViewModel
    {
        private readonly MenuPageService _service = new MenuPageService();

        /* ===================== MENU ===================== */

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

        public decimal TotalAmount =>
            CartItems.Sum(x => x.SubTotal);

        private CartItem FindCartItem(MenuItem item)
        {
            return CartItems.FirstOrDefault(c => c.Item.menuItemId == item.menuItemId);
        }

        /* ===================== COMMANDS ===================== */

        public ICommand AddToCartCommand { get; }
        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }
        public ICommand RemoveItemCommand { get; }

        public ICommand SelectAllCommand { get; }
        public ICommand SelectBeefCommand { get; }
        public ICommand SelectPorkCommand { get; }
        public ICommand SelectChickenCommand { get; }
        public ICommand SelectAnotherCommand { get; }
        public ICommand SelectDrinkCommand { get; }

        public ICommand OpenInsertMenuItemFormCommand { get; }
        public ICommand OpenInsertCustomerFormCommand { get; }

        /* ===================== CONSTRUCTOR ===================== */

        public MenuPageViewModel()
        {
            AddToCartCommand = new RelayCommand(AddToCart);
            IncreaseCommand = new RelayCommand(Increase);
            DecreaseCommand = new RelayCommand(Decrease);
            RemoveItemCommand = new RelayCommand(RemoveCartItem);

            SelectAllCommand = new RelayCommand(_ => Type = null);
            SelectBeefCommand = new RelayCommand(_ => Type = "Thịt bò");
            SelectPorkCommand = new RelayCommand(_ => Type = "Thịt heo");
            SelectChickenCommand = new RelayCommand(_ => Type = "Thịt gà");
            SelectAnotherCommand = new RelayCommand(_ => Type = "Món khác");
            SelectDrinkCommand = new RelayCommand(_ => Type = "Nước uống");

            OpenInsertMenuItemFormCommand = new RelayCommand(OpenInsertMenuItemForm);
            OpenInsertCustomerFormCommand = new RelayCommand(OpenInsertCustomerForm);

            LoadMenuItems();
        }

        /* ===================== CART LOGIC ===================== */

        private void SyncMenuItemQuantity(MenuItem item, int quantity)
        {
            item.DisplayQuantity = quantity;
        }
        private void AddToCart(object o)
        {
            var item = o as MenuItem;
            if (item == null) return;

            var cartItem = FindCartItem(item);
            if (cartItem == null)
            {
                cartItem = new CartItem(item);
                CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }

            SyncMenuItemQuantity(item, cartItem.Quantity);
            RaiseCartChanged();
        }



        private void Increase(object o)
        {
            var item = o as MenuItem;
            if (item == null) return;

            var cartItem = FindCartItem(item);
            if (cartItem == null) return;

            cartItem.Quantity++;
            SyncMenuItemQuantity(item, cartItem.Quantity);
            RaiseCartChanged();
        }


        private void Decrease(object o)
        {
            var item = o as MenuItem;
            if (item == null) return;

            var cartItem = FindCartItem(item);
            if (cartItem == null) return;

            cartItem.Quantity--;

            if (cartItem.Quantity <= 0)
            {
                CartItems.Remove(cartItem);
                SyncMenuItemQuantity(item, 0);
            }
            else
            {
                SyncMenuItemQuantity(item, cartItem.Quantity);
            }

            RaiseCartChanged();
        }

        private void RemoveCartItem(object o)
        {
            if (o is CartItem cartItem)
            {
                CartItems.Remove(cartItem);
                SyncMenuItemQuantity(cartItem.Item, 0);
                RaiseCartChanged();
            }
        }

        private void RaiseCartChanged()
        {
            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(TotalAmount));
        }

        /* ===================== LOAD MENU ===================== */

        private void LoadMenuItems()
        {
            var result = _service.GetMenuItems(Type, SearchText);
            if (!result.Success) return;

            MenuItems.Clear();
            foreach (var item in result.MenuItemList)
            {
                var cartItem = FindCartItem(item);
                MenuItems.Add(item);
                item.DisplayQuantity = cartItem?.Quantity ?? 0;
            }
        }

        /* ===================== OPEN FORM ===================== */

        private void OpenInsertMenuItemForm(object obj)
        {
            var vm = new InsertMenuItemViewModel();
            var window = new InsertMenuItemForm { DataContext = vm };
            vm.CloseAction = () => window.Close();
            window.ShowDialog();
        }

        private void OpenInsertCustomerForm(object obj)
        {
            var vm = new InsertCustomerViewModel();
            var window = new InsertCustomerWindow { DataContext = vm };
            vm.CloseAction = () => window.Close();
            window.ShowDialog();
        }
    }
}
