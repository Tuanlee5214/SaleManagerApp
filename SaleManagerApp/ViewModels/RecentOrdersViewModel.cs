using SaleManagerApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using SaleManagerApp.Views;

namespace SaleManagerApp.ViewModels
{
    public class RecentOrdersViewModel : BaseViewModel
    {
        private readonly MenuPageService service = new MenuPageService();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();

            }
        }

        public ObservableCollection<RecentOrderItem> Orders { get; set; } = new ObservableCollection<RecentOrderItem>();

        // Command cho click vào Status Border
        public ICommand ChangeStatusCommand { get; }

        // Command cho click vào phần Order khác để mở detail
        public ICommand OpenOrderDetailCommand { get; }

        public RecentOrdersViewModel()
        {
            var orders = service.GetAllOrder();
            foreach (var c in orders.listorder)
                Orders.Add(c);

            ChangeStatusCommand = new RelayCommand(ChangeStatus);
            OpenOrderDetailCommand = new RelayCommand(OpenOrderDetail);
        }

        private void ChangeStatus(object parameter)
        {
            if (parameter is RecentOrderItem order)
            {
                string newStatus = string.Empty;

                if (order.ServeStatus == "Đang chế biến")
                {
                    if (MessageBox.Show("Chuyển sang 'Sẵn sàng'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        newStatus = "Sẵn sàng";
                }
                else if (order.ServeStatus == "Sẵn sàng")
                {
                    if (MessageBox.Show("Chuyển sang 'Đã phục vụ'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        newStatus = "Đã phục vụ";
                }

                if (!string.IsNullOrEmpty(newStatus))
                {
                    // Cập nhật Database
                    service.UpdateServeStatus(order.OrderId, newStatus);

                    // Cập nhật Property -> Setter sẽ tự động gọi OnPropertyChanged
                    order.ServeStatus = newStatus;

                    OrderBroadcaster.NotifyUpdate();
                }
            }
        }

        private void OpenOrderDetail(object parameter)
        {
            //MessageBox.Show("Bạn có muốn xem chi tiết không");
            if (parameter is RecentOrderItem order)
            {
                var orderDetail = service.GetOrderDetailById(order.OrderId);
                var vm = new OrderDetailViewModelWin(order, orderDetail);

                var detailWindow = new OrderDetailWindow();
                detailWindow.DataContext = vm;
                detailWindow.ShowDialog();
            }    
        }

        // Nếu muốn trigger UI refresh StatusText & StatusColor
        private void RaiseOrdersChanged(RecentOrderItem order)
        {
            var idx = Orders.IndexOf(order);
            if (idx >= 0)
            {
                Orders[idx] = Orders[idx]; // trigger binding refresh
            }
        }
    }
}
