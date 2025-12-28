using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using SaleManagerApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class MenuPageViewModel : BaseViewModel
    {
        private readonly MenuPageService _service = new MenuPageService();
        /* ===================== Order ===================== */
        public enum OrderType
        {
            TakeAway = 0,   // Mang đi
            DineIn = 1      // Ăn tại bàn
        }
        string orderIdForInvoice = "";
        decimal totalAmountForInvoice = 0;
        public class OrderTypeItem
        {
            public OrderType Value { get; set; }   // dùng lưu DB
            public string Display { get; set; }    // hiển thị tiếng Việt
        }
        public ObservableCollection<OrderTypeItem> OrderTypes { get; }
            = new ObservableCollection<OrderTypeItem>
            {
        new OrderTypeItem
        {
            Value = OrderType.DineIn,
            Display = "Ăn tại bàn"
        },
        new OrderTypeItem
        {
            Value = OrderType.TakeAway,
            Display = "Mang đi"
        }
            };

        private OrderTypeItem _selectedOrderType;
        public OrderTypeItem SelectedOrderType
        {
            get => _selectedOrderType;
            set
            {
                _selectedOrderType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsTableSelectionEnabled));

                // Không ăn tại bàn thì bỏ chọn bàn
                if (_selectedOrderType?.Value != OrderType.DineIn)
                    SelectedTableId = null;
            }
        }

        public bool IsTableSelectionEnabled
        {
            get => SelectedOrderType?.Value == OrderType.DineIn;
        }


        public ObservableCollection<Table> Tables { get; }
    = new ObservableCollection<Table>();

        private string _selectedTableId;
        public string SelectedTableId
        {
            get => _selectedTableId;
            set
            {
                _selectedTableId = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<RecentOrderItem> RecentOrders { get; set; } = new ObservableCollection<RecentOrderItem>();


        /* ===================== MENU ===================== */
        private string _selectedType;
        public string SelectedType
        {
            get => _selectedType;
            set { _selectedType = value; OnPropertyChanged(); }
        }
        public ObservableCollection<MenuItem> MenuItems { get; }
            = new ObservableCollection<MenuItem>();

        private string _currentOrderId;
        public string CurrentOrderId
        {
            get => _currentOrderId;
            set { _currentOrderId = value; OnPropertyChanged(); }
        }


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
        public ICommand SaveOrder { get; }

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
            SaveOrder = new RelayCommand(SaveOrders);
            CurrentOrderId = _service.GetOrderId();
            LoadTables();

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

            OrderBroadcaster.OnOrdersUpdated += () => {
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    LoadTop3Orders();
                });
            };
            LoadMenuItems();
            LoadTop3Orders();
            
        }


        private void LoadTop3Orders()
        {
            RecentOrders.Clear();
            var orders = _service.GetTop3Order();
            foreach (var o in orders.listorder)
                RecentOrders.Add(o);
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
            if (o is CartItem cartItem)
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

        //Table
        public void LoadTables()
        {
            var result = _service.GetAvailabeTable();

            Tables.Clear();
            foreach (var table in result.TableList)
                Tables.Add(table);
        }

        private List<OrderDetail> BuildOrderDetails()
        {
            return CartItems.Select(c => new OrderDetail
            {
                orderId = CurrentOrderId,
                menuItemId = c.MenuItemId,
                quantity = c.Quantity,
                currentPrice = c.UnitPrice,
                createdAt = DateTime.Now
            }).ToList();
        }

        public void SaveOrders(object o)
        {
            bool aa = false;
            orderIdForInvoice = CurrentOrderId;
            totalAmountForInvoice = TotalAmount;

            string orderStatusString;
            if (SelectedOrderType.Value == OrderType.DineIn)
            {
                orderStatusString = "Ăn tại bàn";
            }
            else
                orderStatusString = "Mang đi";

            // 1. Insert Order
            var result1 = _service.InsertOrder(new Order
            {
                orderId = CurrentOrderId,
                orderStatus = orderStatusString,
                serveStatus = "Đang chế biến",
                tableId = SelectedTableId,
                createdAt = DateTime.Now
            });
            if (result1.Success)
            {
                ToastService.Show(result1.SuccessMessage);
                aa = true;
            }
            else
            {
                ToastService.ShowError(result1.ErrorMessage);
                aa = true;
            }

            // 2. Insert OrderDetails
            var details = BuildOrderDetails();
            var result = _service.InsertOrderDetail(details);

            if (result.Success && !aa)
            {
                ToastService.Show(result.SuccessMessage);
            }
            else if (!result.Success && !aa)
                ToastService.ShowError(result.ErrorMessage);


            // 3. Clear cart
            CartItems.Clear();
            RaiseCartChanged();
            foreach (var menu in MenuItems)
            {
                menu.DisplayQuantity = 0;
            }
            foreach (var menu in MenuItems)
                OnPropertyChanged(nameof(menu.DisplayQuantity));
            CurrentOrderId = _service.GetOrderId();
            

            var invoiceVM = new InvoiceViewModel(orderIdForInvoice, totalAmountForInvoice);
            var invoiceWindow = new InvoiceWindow { DataContext = invoiceVM };
            invoiceWindow.ShowDialog();

            RecentOrders.Clear();
            var orders = _service.GetTop3Order(); // trả về List<RecentOrderItem>
            foreach (var c in orders.listorder)
                RecentOrders.Add(c);
        }


    }

    public static class OrderBroadcaster
    {
        public static event Action OnOrdersUpdated;
        public static void NotifyUpdate() => OnOrdersUpdated?.Invoke();
    }
    public class RecentOrderItem : INotifyPropertyChanged
    {
        public string OrderId { get; set; }

        private string _serveStatus;
        public string ServeStatus
        {
            get => _serveStatus;
            set
            {
                if (_serveStatus != value)
                {
                    _serveStatus = value;
                    // Thông báo ServeStatus thay đổi
                    OnPropertyChanged(nameof(ServeStatus));
                    // Thông báo các thuộc tính phụ thuộc thay đổi để UI vẽ lại màu/chữ
                    OnPropertyChanged(nameof(StatusColor));
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }

        public int ItemCount { get; set; }

        public string StatusColor
        {
            get
            {
                if (ServeStatus == "Sẵn sàng") return "#4CAF50";
                if (ServeStatus == "Đang chế biến") return "#FF9800";
                if (ServeStatus == "Đã phục vụ") return "#2196F3";
                return "#999";
            }
        }

        public string StatusText
        {
            get
            {
                if (ServeStatus == "Sẵn sàng") return "Sẵn sàng ✓";
                if (ServeStatus == "Đang chế biến") return "Đang chế biến ⟳";
                if (ServeStatus == "Đã phục vụ") return "Đã phục vụ ✔";
                return ServeStatus;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
