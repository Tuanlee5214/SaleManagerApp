using System;
using System.Windows;
using System.Windows.Controls;
using SaleManagerApp.Services;
using SaleManagerApp.ViewModels;

namespace SaleManagerApp.Views
{
    public partial class HomePage : UserControl
    {
        private HomePageViewModel _viewModel;

        public HomePage()
        {
            InitializeComponent();
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            try
            {
                var dbService = new DBConnectionService();
                string connectionString = dbService.GetConnectionString();
                var comboService = new ComboService(connectionString);
                var comboAnalyzer = new ComboAnalyzer();

                _viewModel = new HomePageViewModel(comboService, comboAnalyzer);
                this.DataContext = _viewModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo: {ex.Message}");
            }
        }

        private async void RefreshCombo_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null) await _viewModel.RefreshCombosAsync();
        }
    }
}