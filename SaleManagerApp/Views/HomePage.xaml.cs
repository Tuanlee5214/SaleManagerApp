using System;
using System.Windows;
using System.Windows.Controls;
using SaleManagerApp.Services;
using SaleManagerApp.ViewModels;

namespace SaleManagerApp.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        private HomePageViewModel _viewModel;

        public HomePage()
        {
            InitializeComponent();

            // Khởi tạo services và ViewModel
            InitializeViewModel();
        }

        /// <summary>
        /// Khởi tạo ViewModel với Dependency Injection thủ công
        /// </summary>
        private void InitializeViewModel()
        {
            try
            {
                Console.WriteLine("🔧 Initializing HomePage ViewModel...");

                // Sử dụng DBConnectionService có sẵn
                var dbService = new DBConnectionService();

                // Lấy connection string từ DBConnectionService
                string connectionString = dbService.GetConnectionString();
                Console.WriteLine($"✅ Connection string obtained: {connectionString.Substring(0, 30)}...");

                // Tạo services
                var comboService = new ComboService(connectionString);
                var comboAnalyzer = new ComboAnalyzer();
                Console.WriteLine("✅ Services created successfully");

                // Inject vào ViewModel
                _viewModel = new HomePageViewModel(comboService, comboAnalyzer);
                Console.WriteLine("✅ HomePageViewModel created");

                // Set DataContext để binding hoạt động
                this.DataContext = _viewModel;
                Console.WriteLine("✅ DataContext set successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error initializing HomePage ViewModel: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");

                // Hiển thị thông báo lỗi
                MessageBox.Show(
                    $"Lỗi khởi tạo trang chủ:\n{ex.Message}\n\nVui lòng kiểm tra kết nối database.",
                    "Lỗi khởi tạo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                // Fallback: Set DataContext với dữ liệu mặc định để tránh crash
                this.DataContext = new
                {
                    BannerText = "KHUYẾN MÃI ĐẶC BIỆT - ĐANG TẢI DỮ LIỆU",
                    Combo1Name = "Combo đang cập nhật",
                    Combo1Price = "Đang phân tích...",
                    Combo1ImageUrl = "/Assets/Images/default-combo.png",
                    Combo2Name = "Combo đang cập nhật",
                    Combo2Price = "Đang phân tích...",
                    Combo2ImageUrl = "/Assets/Images/default-combo.png",
                    Combo3Name = "Combo đang cập nhật",
                    Combo3Price = "Đang phân tích...",
                    Combo3ImageUrl = "/Assets/Images/default-combo.png",
                    IsLoading = false
                };

                Console.WriteLine("⚠️ Fallback DataContext set with default values");
            }
        }

        /// <summary>
        /// Event handler cho button Refresh (nếu có)
        /// </summary>
        private async void RefreshCombo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel != null)
                {
                    Console.WriteLine("🔄 Refreshing combos...");
                    await _viewModel.RefreshCombosAsync();
                    Console.WriteLine("✅ Combos refreshed successfully");
                }
                else
                {
                    Console.WriteLine("⚠️ ViewModel is null, cannot refresh");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error refreshing combos: {ex.Message}");
                MessageBox.Show(
                    $"Lỗi khi làm mới combo:\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        /// <summary>
        /// Xử lý khi UserControl được load
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("📄 HomePage loaded");

            // Log trạng thái ViewModel
            if (_viewModel != null)
            {
                Console.WriteLine("✅ ViewModel is initialized");
            }
            else
            {
                Console.WriteLine("⚠️ ViewModel is NULL - DataContext may be anonymous object");
            }
        }
    }
}