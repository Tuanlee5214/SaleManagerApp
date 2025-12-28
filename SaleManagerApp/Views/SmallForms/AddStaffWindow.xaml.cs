using SaleManagerApp.ViewModels;
using System.Windows;

namespace SaleManagerApp.Views
{
    public partial class AddStaffWindow : Window
    {
        private AddStaffViewModel _viewModel;
        public AddStaffViewModel ViewModel => _viewModel;

        public AddStaffWindow(string defaultPosition = null)
        {
            InitializeComponent();
            _viewModel = new AddStaffViewModel(defaultPosition);
            DataContext = _viewModel;

            _viewModel.CloseAction = () =>
            {
                this.Close();
            };

            // Đảm bảo Window này active để ToastService nhận diện
            this.Activated += (s, e) => this.Activate();
        }
    }
}