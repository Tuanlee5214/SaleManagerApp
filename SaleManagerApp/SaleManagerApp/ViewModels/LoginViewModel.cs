using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SaleManagerApp.Services;
using System.Windows.Input;
using SaleManagerApp.Navigation;
using System.Reflection;
using System.Windows.Controls;
using SaleManagerApp.Helpers;
using System.Windows;

namespace SaleManagerApp.ViewModels
{
    public class LoginViewModel: BaseViewModel
    {
        private readonly UserService _userService = new UserService();

        public string Username { get; set; }

        private string _errorMessage;
        private string _successMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set { _successMessage = value; OnPropertyChanged(); }
        }


        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            //LoginCommand = new RelayCommand(async (obj) => await LoginAsync(obj));
        }

        private bool _overlayVisible;
        public bool OverlayVisible
        {
            get => _overlayVisible;
            set { _overlayVisible = value; OnPropertyChanged(); }
        }


        public async Task LoginAsync(string Password)
        {
            OverlayVisible = true;

            ErrorMessage = " ";
            SuccessMessage = " ";

            var result = await Task.Run(() =>
            {
                return _userService.Login(Username, Password);
            });

            OverlayVisible = false; 

            if (!result.Success)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ToastService.ShowErrorLogin(result.ErrorMessage);
                });

                return;
            }

            UserSession.SetUser(result.user);
            SuccessMessage = result.SuccesMessage;

            NavigationService.Navigate(new MainLayoutViewModel());
        }
    }
}
