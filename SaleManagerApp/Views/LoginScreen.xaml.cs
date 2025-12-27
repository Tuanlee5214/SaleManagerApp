using SaleManagerApp.Helpers;
using SaleManagerApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SaleManagerApp.Views
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : UserControl
    {
        public LoginScreen()
        {
            InitializeComponent();
            ToastService.Initialize(this.RootGrid);
            //this.DataContext = new LoginViewModel();
        }

        private void chkShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            pwdVisible.Text = pwdHidden.Password;
            pwdVisible.Visibility = Visibility.Visible;
            pwdHidden.Visibility = Visibility.Collapsed;
        }

        private void chkShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            pwdHidden.Password = pwdVisible.Text;
            pwdHidden.Visibility = Visibility.Visible;
            pwdVisible.Visibility = Visibility.Collapsed;
        }

        private void pwdHidden_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pwdVisible.Visibility == Visibility.Visible)
            {
                pwdVisible.Text = pwdHidden.Password;
            }
        }

        private void pwdVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (pwdHidden.Visibility == Visibility.Visible)
            {
                pwdHidden.Password = pwdVisible.Text;
            }
        }

        private string GetCurrentPassword()
        {
            return (pwdHidden.Visibility == Visibility.Visible) ? pwdHidden.Password : pwdVisible.Text;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string password = GetCurrentPassword();

            var vm = DataContext as LoginViewModel;
            vm?.LoginAsync(password);
        }


        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Tính năng đang phát triển.");
        }
    }
}
