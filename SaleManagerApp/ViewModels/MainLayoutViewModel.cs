using SaleManagerApp.Helpers;
using SaleManagerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SaleManagerApp.Views;

namespace SaleManagerApp.ViewModels
{
    public class MainLayoutViewModel : BaseViewModel
    {
        private string _selectedMenu = "Home";
        public string SelectedMenu
        {
            get => _selectedMenu;
            set { _selectedMenu = value; OnPropertyChanged(); }
        }


        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value; OnPropertyChanged();

                IsVisibleToastLoginSuccess();
            }
        }
        public void IsVisibleToastLoginSuccess()
        {
            if (CurrentViewModel is HomePageViewModel && UserSession.JustLoggedIn)
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ToastService.Show("Đăng nhập thành công!");
                    UserSession.JustLoggedIn = false;
                }, System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        public ICommand ShowHomeCommand { get; }
        public ICommand ShowMenuCommand { get; }

        public ICommand ShowUserCommand { get; }

        public ICommand ShowWareHouseCommand { get; }
        public MainLayoutViewModel()
        {
            // Trang mặc định
            CurrentViewModel = new HomePageViewModel();
            SelectedMenu = "Home";

            ShowHomeCommand = new RelayCommand(o =>
            {
                CurrentViewModel = new HomePageViewModel();
                SelectedMenu = "Home";
                Console.WriteLine("SELECTED = HOME");

            });
            ShowMenuCommand = new RelayCommand(o => {
                CurrentViewModel = new MenuPageViewModel();
                SelectedMenu = "Menu";
                Console.WriteLine("SELECTED = menu");

            });
            ShowUserCommand = new RelayCommand(o => {
                CurrentViewModel = new UserPageViewModel();
                SelectedMenu = "Staff";
                Console.WriteLine("SELECTED = user");

            });
            ShowWareHouseCommand = new RelayCommand(o => {
                CurrentViewModel = new WarehousePageViewModel();
                SelectedMenu = "WareHouse";
                Console.WriteLine("SELECTED = warehouse");

            });

        }
    }
}