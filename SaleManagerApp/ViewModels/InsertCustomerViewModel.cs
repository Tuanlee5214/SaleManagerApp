using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class InsertCustomerViewModel : BaseViewModel
    {
        private readonly MenuPageService _service = new MenuPageService();

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        public ICommand InsertCustomerCommand { get; }
        public ICommand CancelFormCommand { get; }

        public InsertCustomerViewModel()
        {
            InsertCustomerCommand = new RelayCommand(obj => InsertCustomer());
            CancelFormCommand = new RelayCommand(obj => CancelForm());
        }

        public Action CloseAction { get; set; }
        public Action ReloadCustomer { get; set; }

        public void CancelForm()
        {
            CloseAction?.Invoke();
        }

        public void InsertCustomer()
        {
            Customer c = new Customer
            {
                fullName = this.FullName,
                phone = this.Phone,
                email = this.Email,
                address = this.Address
            };

            var result = _service.InsertCustomer(c);

            if (result.Success)
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ReloadCustomer?.Invoke();
                    ToastService.Show(result.SuccessMessage);
                    CloseAction?.Invoke();
                });
            }
            else
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ToastService.ShowError(result.ErrorMessage);
                });
            }
        }
    }
}
