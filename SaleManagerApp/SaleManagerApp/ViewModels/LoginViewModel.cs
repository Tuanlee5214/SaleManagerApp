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
            LoginCommand = new RelayCommand(Login);
        }

        public void Login(object obj)
        {
            var pwdBox = obj as PasswordBox;
            string Password = pwdBox?.Password ?? "";
            var result = _userService.Login(Username, Password); 

            if(!result.Success)
            {
                ErrorMessage = result.ErrorMessage;
                return;  
            }

            UserSession.SetUser(result.user);
            SuccessMessage = result.SuccesMessage;
            NavigationService.Navigate(new MainLayoutViewModel());
        }

    }
}
