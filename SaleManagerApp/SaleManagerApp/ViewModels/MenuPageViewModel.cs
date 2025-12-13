using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

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

        public ICommand SelectAllCommand { get; }
        public ICommand SelectBeefCommand { get; }
        public ICommand SelectPorkCommand { get; }
        public ICommand SelectChickenCommand { get; }
        public ICommand SelectAnotherCommand { get; }
        public ICommand SelectDrinkCommand { get; }

        /* ===================== CONSTRUCTOR ===================== */

        public MenuPageViewModel()
        {
            AddToCartCommand = new RelayCommand(o => AddToCart(o as MenuItem));
            IncreaseCommand = new RelayCommand(o => Increase(o as MenuItem));
            DecreaseCommand = new RelayCommand(o => Decrease(o as MenuItem));
            RemoveItemCommand = new RelayCommand(RemoveCartItem);

            SelectAllCommand = new RelayCommand(_ => Type = null);
            SelectBeefCommand = new RelayCommand(_ => Type = "Thịt bò");
            SelectPorkCommand = new RelayCommand(_ => Type = "Thịt heo");
            SelectChickenCommand = new RelayCommand(_ => Type = "Thịt gà");
            SelectAnotherCommand = new RelayCommand(_ => Type = "Món khác");
            SelectDrinkCommand = new RelayCommand(_ => Type = "Nước uống");

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

        /* ===================== LOAD MENU ===================== */

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
