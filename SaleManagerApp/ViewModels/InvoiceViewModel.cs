using SaleManagerApp.Helpers;
using SaleManagerApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class InvoiceViewModel :BaseViewModel
    {
        private readonly MenuPageService service = new MenuPageService();
        private readonly string _orderId;
        private readonly decimal _totalAmount;
        public ICommand SaveInvoiceCommand { get; set; }


        public InvoiceViewModel(string orderId, decimal totalAmount)
        {
            
            _orderId = orderId;
            _totalAmount = totalAmount;

            // Sinh InvoiceId mới
            InvoiceId = service.GetInvoiceId();
            SaveInvoiceCommand = new RelayCommand(SaveInvoice);
            OrderId = orderId;
            TotalAmount = totalAmount;

            PaymentMethods = new ObservableCollection<string> { "Chuyển khoản", "Tiền mặt" };
            InvoiceStatuses = new ObservableCollection<string> { "Đã thanh toán", "Chưa thanh toán" };

            // Mặc định
            SelectedPaymentMethod = PaymentMethods[0];
            SelectedInvoiceStatus = InvoiceStatuses[1];
        }

        public string InvoiceId { get; }
        public string OrderId { get; }
        public decimal TotalAmount { get; }

        public ObservableCollection<string> PaymentMethods { get; }
        public string SelectedPaymentMethod { get; set; }

        public ObservableCollection<string> InvoiceStatuses { get; }
        public string SelectedInvoiceStatus { get; set; }

        public void SaveInvoice(object obj)
        {
            var result = service.InsertInvoice(new Models.Invoice
            {
                invoiceId = InvoiceId,
                orderId = OrderId,
                totalAmount = TotalAmount,
                paymentMethod = SelectedPaymentMethod,
                invoiceStatus = SelectedInvoiceStatus
            }
            );

            if(result.Success)
            {
                ToastService.Show(result.SuccessMessage);
                
            }
            else
            {
                ToastService.ShowError(result.ErrorMessage);
            }
        }

    }
}
