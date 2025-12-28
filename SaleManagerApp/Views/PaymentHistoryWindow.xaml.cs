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

namespace SaleManagerApp.Views
{
    /// <summary>
    /// Interaction logic for PaymentHistoryWindow.xaml
    /// </summary>
    public partial class PaymentHistoryWindow : Window
    {
        public PaymentHistoryWindow()
        {
            InitializeComponent();
            this.DataContext = new SaleManagerApp.ViewModels.PayMentHistoryViewModel();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
