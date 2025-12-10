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
    /// Interaction logic for InsertCustomerWindow.xaml
    /// </summary>
    public partial class InsertCustomerWindow : Window
    {
        public InsertCustomerWindow()
        {
            InitializeComponent();

            var vm = new InsertCustomerViewModel();
            vm.CloseAction = () => this.Close();
            this.DataContext = vm;
        }
    }
}
