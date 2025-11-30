using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class MainLayoutViewModel: BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set { _currentViewModel = value; OnPropertyChanged(); }
        }

        public ICommand ShowHomeCommand { get; }
        public ICommand ShowMenuCommand { get; }

        public ICommand ShowUserCommand { get; }

        public ICommand ShowWareHouseCommand { get; }
        public MainLayoutViewModel()
        {
            // Trang mặc định
            CurrentViewModel = new HomePageViewModel();

            ShowHomeCommand = new RelayCommand(o => CurrentViewModel = new HomePageViewModel());
            ShowMenuCommand = new RelayCommand(o => CurrentViewModel = new MenuPageViewModel());
            ShowUserCommand = new RelayCommand(o => CurrentViewModel = new UserPageViewModel());
            ShowWareHouseCommand = new RelayCommand(o => CurrentViewModel = new WareHousePageViewModel());

        }
    }
}
