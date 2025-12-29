using System.Windows;
using SaleManagerApp.ViewModels;

namespace SaleManagerApp.Views
{
    public partial class WarehouseLogView : Window
    {
        public WarehouseLogView(WarehouseLogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}