using System.Windows;
using SaleManagerApp.ViewModels;

namespace SaleManagerApp.Views
{
    public partial class BatchDetailView : Window
    {
        public BatchDetailView(BatchDetailViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}