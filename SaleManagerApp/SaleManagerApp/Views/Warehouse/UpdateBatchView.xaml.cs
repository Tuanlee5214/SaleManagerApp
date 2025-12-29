using System.Windows;
using SaleManagerApp.ViewModels;

namespace SaleManagerApp.Views
{
    public partial class UpdateBatchView : Window
    {
        public UpdateBatchView(UpdateBatchViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}