using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.ViewModels
{
    public class PayMentHistoryViewModel :BaseViewModel
    {
        private readonly MenuPageService _service; 

        private ObservableCollection<Invoice> _payments;
        public ObservableCollection<Invoice> Payments
        {
            get => _payments;
            set
            {
                _payments = value;
                OnPropertyChanged();
            }
        }

        public PayMentHistoryViewModel()
        {
            _service = new MenuPageService();
            Payments = new ObservableCollection<Invoice>();

            LoadPaymentHistory();
        }

        private void LoadPaymentHistory()
        {
            try
            {
                // Gọi xuống Service để lấy dữ liệu từ SQL
                var data = _service.GetInvoices();

                Payments.Clear();
                foreach (var item in data)
                {
                    Payments.Add(item);
                }
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc thông báo nếu cần
                System.Diagnostics.Debug.WriteLine("Lỗi tải lịch sử: " + ex.Message);
            }
        }
    }
}
