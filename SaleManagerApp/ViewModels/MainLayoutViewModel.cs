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

        // ✅ ĐỔI: Từ BaseViewModel → object
        private object _currentPage;
        public object CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                IsVisibleToastLoginSuccess();
            }
        }

        public void IsVisibleToastLoginSuccess()
        {
            // ✅ SỬA: Kiểm tra type của View thay vì ViewModel
            if (_currentPage is HomePage && UserSession.JustLoggedIn)
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
            // ✅ Trang mặc định - Load HomePage View
            CurrentPage = new HomePage();
            SelectedMenu = "Home";

            ShowHomeCommand = new RelayCommand(o =>
            {
                CurrentPage = new HomePage();
                SelectedMenu = "Home";
                Console.WriteLine("✅ SELECTED = HOME");
            });

            ShowMenuCommand = new RelayCommand(o =>
            {
                CurrentPage = new MenuPage(); // ✅ BỎ COMMENT
                SelectedMenu = "Menu";
                Console.WriteLine("✅ SELECTED = MENU");
            });

            ShowUserCommand = new RelayCommand(o =>
            {
                CurrentPage = new UserPage(); // ✅ BỎ COMMENT
                SelectedMenu = "Staff";
                Console.WriteLine("✅ SELECTED = USER");
            });

            ShowWareHouseCommand = new RelayCommand(o =>
            {
                CurrentPage = new WarehousePage(); // ✅ BỎ COMMENT (nếu có class này)
                SelectedMenu = "WareHouse";
                Console.WriteLine("✅ SELECTED = WAREHOUSE");
            });
        }
    }
}