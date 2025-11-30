using SaleManagerApp.Navigation;
using SaleManagerApp.ViewModels;
using SaleManagerApp.Views;
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
using System.Windows.Shapes;

namespace SaleManagerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var mainVM = new MainViewModel();
            DataContext = mainVM;

            NavigationService.NavigateAction = (vm) =>
            {
                mainVM.CurrentViewModel = vm;
            };

            mainVM.CurrentViewModel = new LoginViewModel();
            
        }
    }
}
