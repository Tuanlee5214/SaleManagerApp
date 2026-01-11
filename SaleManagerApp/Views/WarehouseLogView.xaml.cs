
using System.Windows;
using System.Windows.Controls;
using SaleManagerApp.ViewModels;

namespace SaleManagerApp.Views
{
    public partial class WarehouseLogView : Window
    {
        public WarehouseLogView()
        {
            InitializeComponent();
        }

        private void FilterType_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && DataContext is WarehouseLogViewModel vm)
            {
                vm.FilterType = rb.Tag?.ToString();
            }
        }
    }
}
