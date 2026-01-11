using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using LiveCharts;
using LiveCharts.Wpf;

namespace SaleManagerApp.ViewModels
{
    public class HomePageViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly ComboService _comboService;
        private readonly ComboAnalyzer _comboAnalyzer;

        // Biểu đồ
        public SeriesCollection RevenueSeries { get; set; }
        public List<string> RevenueLabels { get; set; }
        public SeriesCollection FoodPieSeries { get; set; }
        public SeriesCollection DrinkPieSeries { get; set; }
        public Func<double, string> YFormatter { get; set; }

        // Properties cho Combos
        private string _combo1Name, _combo1Price, _combo1ImageUrl;
        private string _combo2Name, _combo2Price, _combo2ImageUrl;
        private string _combo3Name, _combo3Price, _combo3ImageUrl;
        private string _bannerText;
        private bool _isLoading;

        public string Combo1Name { get => _combo1Name; set { _combo1Name = value; OnPropertyChanged(); } }
        public string Combo1Price { get => _combo1Price; set { _combo1Price = value; OnPropertyChanged(); } }
        public string Combo1ImageUrl { get => _combo1ImageUrl; set { _combo1ImageUrl = value; OnPropertyChanged(); } }
        public string Combo2Name { get => _combo2Name; set { _combo2Name = value; OnPropertyChanged(); } }
        public string Combo2Price { get => _combo2Price; set { _combo2Price = value; OnPropertyChanged(); } }
        public string Combo2ImageUrl { get => _combo2ImageUrl; set { _combo2ImageUrl = value; OnPropertyChanged(); } }
        public string Combo3Name { get => _combo3Name; set { _combo3Name = value; OnPropertyChanged(); } }
        public string Combo3Price { get => _combo3Price; set { _combo3Price = value; OnPropertyChanged(); } }
        public string Combo3ImageUrl { get => _combo3ImageUrl; set { _combo3ImageUrl = value; OnPropertyChanged(); } }
        public string BannerText { get => _bannerText; set { _bannerText = value; OnPropertyChanged(); } }
        public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(); } }

        // SỬA LỖI CS1729: Constructor nhận 2 tham số
        public HomePageViewModel(ComboService comboService, ComboAnalyzer comboAnalyzer)
        {
            _comboService = comboService;
            _comboAnalyzer = comboAnalyzer;

            InitCharts();
            _ = LoadCombosAsync();
        }

       

        // THÊM Constructor mặc định này để tránh lỗi CS7036 ở các file khác
        public HomePageViewModel()
        {
            // Khởi tạo mặc định nếu cần
            InitCharts();
        }

        private void InitCharts()
        {
            RevenueSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Doanh thu",
                    Values = new ChartValues<decimal> { 8219000, 390000, 4500000, 3000000, 5000000 },
                    Stroke = new SolidColorBrush(Color.FromRgb(77, 122, 164)),
                    Fill = Brushes.Transparent
                }
            };
            RevenueLabels = new List<string> { "25/12", "26/12", "27/12", "28/12", "29/01" };
            YFormatter = value => value.ToString("N0") + "đ";

            FoodPieSeries = new SeriesCollection
            {
                new PieSeries { Title = "Gà rim nước mắm", Values = new ChartValues<double> { 7 }, DataLabels = true },
                new PieSeries { Title = "Thịt heo xào xả ớt", Values = new ChartValues<double> { 15 }, DataLabels = true },
                new PieSeries { Title = "Thịt bò nướng BBQ", Values = new ChartValues<double> {  18}, DataLabels = true },
                new PieSeries { Title = "Thịt kho tàu", Values = new ChartValues<double> { 4 }, DataLabels = true },
                new PieSeries { Title = "Thịt bò hầm củ quả", Values = new ChartValues<double> { 22 }, DataLabels = true },

                new PieSeries { Title = "Khác", Values = new ChartValues<double> { 41 }, DataLabels = true }
            };

            DrinkPieSeries = new SeriesCollection
            {
                new PieSeries { Title = "Trà sữa kem chứng muối", Values = new ChartValues<double> { 9 }, DataLabels = true },
                new PieSeries { Title = "Cà phê sữa", Values = new ChartValues<double> { 20 }, DataLabels = true },
                new PieSeries { Title = "Matcha Latte", Values = new ChartValues<double> { 16 }, DataLabels = true },
                new PieSeries { Title = "Khác", Values = new ChartValues<double> { 14 }, DataLabels = true },
    
            };
        }

        public async Task LoadCombosAsync()
        {
            try
            {
                IsLoading = true;
                var activeCombos = await _comboService.GetActiveCombosAsync();
                if (activeCombos.Count >= 3) DisplayCombos(activeCombos);
                else DisplayDefaultCombos();
            }
            catch { DisplayDefaultCombos(); }
            finally { IsLoading = false; }
        }

        // SỬA LỖI CS1061: Thêm phương thức RefreshCombosAsync
        public async Task RefreshCombosAsync()
        {
            await LoadCombosAsync();
        }

        private void DisplayCombos(List<ComboPromotion> combos)
        {
            Combo1Name = combos[0].ComboName; Combo1Price = combos[0].FormattedPrice;
            Combo2Name = combos[1].ComboName; Combo2Price = combos[1].FormattedPrice;
            Combo3Name = combos[2].ComboName; Combo3Price = combos[2].FormattedPrice;
            BannerText = "KHUYẾN MÃI HÔM NAY";
        }

        private void DisplayDefaultCombos()
        {
            BannerText = "CHÀO MỪNG BẠN";
            Combo1Name = "Đang cập nhật..."; Combo1Price = "0đ";
        }

        // SỬA CẢNH BÁO CS0108: Dùng từ khóa 'new' nếu BaseViewModel đã có các thành viên này
        public new event PropertyChangedEventHandler PropertyChanged;
        protected new void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}