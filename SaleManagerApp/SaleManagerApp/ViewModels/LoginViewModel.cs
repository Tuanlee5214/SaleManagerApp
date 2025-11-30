using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SaleManagerApp.Services;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SaleManagerApp.ViewModels
{
    public class LoginViewModel: BaseViewModel
    {
        private readonly UserService _userService = new UserService();

        public string Username { get; set; }
        public string Password { get; set; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }
        
        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        public void Login(object obj)
        {
            var user = _userService.Login(Username, Password);

            if(user == null)
            {
                ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
                return;
            }

            UserSession.SetUser(user);

            NavigationService.Navigate(new HomePageViewModel());
        }

    }
}
