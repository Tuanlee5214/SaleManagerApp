using System.Windows;
using SaleManagerApp.Model;
using SaleManagerApp.ViewModels;

namespace SaleManagerApp.Views
{
    public partial class EditStaffWindow : Window
    {
        private EditStaffViewModel _viewModel;

        public EditStaffWindow(Staff staff)
        {
            InitializeComponent();

            _viewModel = new EditStaffViewModel(staff);
            DataContext = _viewModel;

            // Chỉ đóng cửa sổ khi Cancel
            _viewModel.CloseAction = () => this.Close();

            // Khi save thành công, set DialogResult = true RỒI MỚI đóng
            _viewModel.SaveSuccessAction = () =>
            {
                this.DialogResult = true;
                // Không cần gọi Close() vì DialogResult = true tự động đóng window
            };

            // Đảm bảo window được activate để ToastService phát hiện đúng
            this.Activated += (s, e) => this.Activate();
            this.Loaded += (s, e) => this.Focus();
        }
    }
}