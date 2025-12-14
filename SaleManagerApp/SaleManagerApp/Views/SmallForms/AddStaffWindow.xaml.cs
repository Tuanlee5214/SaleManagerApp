using SaleManagerApp.ViewModels;
using System.Windows;

namespace SaleManagerApp.Views
{
    public partial class AddStaffWindow : Window
    {
        private AddStaffViewModel _viewModel;

        public AddStaffViewModel ViewModel => _viewModel;

        public AddStaffWindow()
        {
            InitializeComponent();

            _viewModel = new AddStaffViewModel();
            DataContext = _viewModel;

            _viewModel.CloseAction = () =>
            {
                this.Close();
            };
        }
    }
}