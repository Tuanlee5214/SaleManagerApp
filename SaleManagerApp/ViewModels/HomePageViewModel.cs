using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SaleManagerApp.Models;
using SaleManagerApp.Services;

namespace SaleManagerApp.ViewModels
{
    public class HomePageViewModel : BaseViewModel, INotifyPropertyChanged
    {
        // Services
        private readonly ComboService _comboService;
        private readonly ComboAnalyzer _comboAnalyzer;

        // Properties cho 3 combo
        private string _combo1Name;
        private string _combo1Price;
        private string _combo1ImageUrl;

        private string _combo2Name;
        private string _combo2Price;
        private string _combo2ImageUrl;

        private string _combo3Name;
        private string _combo3Price;
        private string _combo3ImageUrl;

        private string _bannerText;
        private bool _isLoading;

        public string Combo1Name
        {
            get => _combo1Name;
            set { _combo1Name = value; OnPropertyChanged(); }
        }

        public string Combo1Price
        {
            get => _combo1Price;
            set { _combo1Price = value; OnPropertyChanged(); }
        }

        public string Combo1ImageUrl
        {
            get => _combo1ImageUrl;
            set { _combo1ImageUrl = value; OnPropertyChanged(); }
        }

        public string Combo2Name
        {
            get => _combo2Name;
            set { _combo2Name = value; OnPropertyChanged(); }
        }

        public string Combo2Price
        {
            get => _combo2Price;
            set { _combo2Price = value; OnPropertyChanged(); }
        }

        public string Combo2ImageUrl
        {
            get => _combo2ImageUrl;
            set { _combo2ImageUrl = value; OnPropertyChanged(); }
        }

        public string Combo3Name
        {
            get => _combo3Name;
            set { _combo3Name = value; OnPropertyChanged(); }
        }

        public string Combo3Price
        {
            get => _combo3Price;
            set { _combo3Price = value; OnPropertyChanged(); }
        }

        public string Combo3ImageUrl
        {
            get => _combo3ImageUrl;
            set { _combo3ImageUrl = value; OnPropertyChanged(); }
        }

        public string BannerText
        {
            get => _bannerText;
            set { _bannerText = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public HomePageViewModel()
        {
            // TODO: Lấy connection string từ config
            string connectionString = "YOUR_CONNECTION_STRING_HERE";

            // Khởi tạo services
            _comboService = new ComboService(connectionString);
            _comboAnalyzer = new ComboAnalyzer();

            // Load combo khi khởi tạo
            _ = LoadCombosAsync();
        }

        /// <summary>
        /// Constructor với dependency injection (khuyến nghị)
        /// </summary>
        public HomePageViewModel(ComboService comboService, ComboAnalyzer comboAnalyzer)
        {
            _comboService = comboService;
            _comboAnalyzer = comboAnalyzer;

            _ = LoadCombosAsync();
        }

        /// <summary>
        /// Load combo từ database hoặc tạo mới nếu chưa có
        /// </summary>
        public async Task LoadCombosAsync()
        {
            try
            {
                IsLoading = true;

                // Kiểm tra kết nối database
                if (!await _comboService.TestConnectionAsync())
                {
                    Console.WriteLine("❌ Không thể kết nối database");
                    DisplayDefaultCombos();
                    return;
                }

                // Bước 1: Kiểm tra xem có combo active cho khung giờ hiện tại không
                var activeCombos = await _comboService.GetActiveCombosAsync();

                if (activeCombos.Count >= 3)
                {
                    // Đã có combo sẵn -> hiển thị
                    Console.WriteLine($"✅ Load {activeCombos.Count} combo từ database");
                    DisplayCombos(activeCombos);
                    UpdateBanner(activeCombos[0]);
                }
                else
                {
                    // Chưa có combo -> phân tích và tạo mới
                    Console.WriteLine("🔄 Chưa có combo, bắt đầu phân tích...");
                    await AnalyzeAndCreateCombosAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi load combo: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                DisplayDefaultCombos();
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// PHẦN QUAN TRỌNG: Phân tích và tạo combo mới
        /// Sử dụng ComboService để giao tiếp với database
        /// </summary>
        private async Task AnalyzeAndCreateCombosAsync()
        {
            Console.WriteLine("🔍 Bắt đầu phân tích combo...");

            // Bước 1: Tìm khung giờ ế nhất
            var timeSlots = await _comboService.GetLowPerformanceTimeSlotsAsync();

            if (timeSlots.Count == 0)
            {
                Console.WriteLine("⚠️ Không tìm thấy dữ liệu đơn hàng");
                DisplayDefaultCombos();
                return;
            }

            var lowPerformanceSlot = timeSlots.First();
            Console.WriteLine($"📉 Khung giờ ế: {lowPerformanceSlot.DayName}, {lowPerformanceSlot.HourOfDay}:00 ({lowPerformanceSlot.OrderCount} đơn)");

            // Bước 2: Lấy dữ liệu đơn hàng trong khung giờ ế
            var orderItems = await _comboService.GetOrderItemsForComboAsync(
                lowPerformanceSlot.DayOfWeek,
                lowPerformanceSlot.HourOfDay,
                lowPerformanceSlot.HourOfDay + 1
            );

            Console.WriteLine($"📦 Lấy được {orderItems.Count} items từ {orderItems.Select(x => x.OrderId).Distinct().Count()} đơn hàng");

            if (orderItems.Count < 20)
            {
                Console.WriteLine("⚠️ Không đủ dữ liệu để phân tích (cần ít nhất 20 items)");
                DisplayDefaultCombos();
                return;
            }

            // Bước 3: Lấy thông tin món ăn để mapping
            var menuItemDict = await _comboService.GetMenuItemDictionaryAsync();
            Console.WriteLine($"📋 Load {menuItemDict.Count} món ăn từ menu");

            // Bước 4: Chạy thuật toán phân tích combo
            var recommendations = _comboAnalyzer.AnalyzeAndRecommend(orderItems, menuItemDict);

            if (recommendations.Count == 0)
            {
                Console.WriteLine("⚠️ Không tìm thấy combo phù hợp");
                DisplayDefaultCombos();
                return;
            }

            Console.WriteLine($"✅ Tạo được {recommendations.Count} combo recommendations");

            // ✅ BƯỚC MỚI: Lọc chỉ giữ combo có số món BẰNG NHAU (ưu tiên 3 món)
            var filteredRecommendations = _comboAnalyzer.FilterUniformSizeCombo(recommendations);

            Console.WriteLine($"");
            Console.WriteLine($"🎯 Sau khi lọc: {filteredRecommendations.Count} combo đồng kích thước");
            foreach (var rec in filteredRecommendations.Take(3))
            {
                Console.WriteLine($"   - {string.Join(" + ", rec.MenuItemNames)} ({rec.MenuItemIds.Count} món)");
            }
            Console.WriteLine($"");

            // Bước 5: Lấy phân bố giá để tính combo price
            var orderValues = await _comboService.GetOrderValueDistributionAsync(
                lowPerformanceSlot.DayOfWeek,
                lowPerformanceSlot.HourOfDay,
                lowPerformanceSlot.HourOfDay + 1
            );

            Console.WriteLine($"💰 Phân tích {orderValues.Count} giá trị đơn hàng");

            // Bước 6: Lưu top 3 combo vào database (SỬ DỤNG FILTERED)
            var savedCombos = await SaveTopCombosAsync(filteredRecommendations, lowPerformanceSlot, orderValues);

            Console.WriteLine($"✅ Lưu thành công {savedCombos.Count} combo vào database");

            // Bước 7: Hiển thị combo
            DisplayCombos(savedCombos);
            UpdateBanner(savedCombos[0]);

            Console.WriteLine("🎉 Hoàn tất phân tích và hiển thị combo");
        }

        /// <summary>
        /// Lưu top 3 combo vào database
        /// Sử dụng ComboService.SaveCompleteComboAsync
        /// </summary>
        private async Task<List<ComboPromotion>> SaveTopCombosAsync(
     List<ComboRecommendation> recommendations,
     TimeSlotPerformance timeSlot,
     List<decimal> orderValues)
        {
            var savedCombos = new List<ComboPromotion>();

            // Lấy top 3 recommendations tốt nhất
            var topRecommendations = recommendations
                .OrderByDescending(r => r.Confidence)
                .ThenByDescending(r => r.Support)
                .Take(3)
                .ToList();

            if (topRecommendations.Count == 0)
            {
                Console.WriteLine("⚠️ Không có combo recommendations");
                return savedCombos;
            }

            Console.WriteLine($"🎯 Top 3 combo được chọn:");
            for (int i = 0; i < topRecommendations.Count; i++)
            {
                Console.WriteLine($"   {i + 1}. {string.Join(" + ", topRecommendations[i].MenuItemNames)}");
            }
            Console.WriteLine($"");

            // ✅ TÍNH GIÁ ĐỒNG NHẤT dựa trên trung bình giá gốc của 3 combo
            decimal uniformComboPrice = _comboAnalyzer.CalculateUniformComboPriceFromAverageCost(
                topRecommendations,
                orderValues
            );

            Console.WriteLine($"═══════════════════════════════════════");
            Console.WriteLine($"🎯 GIÁ ĐỒNG NHẤT: {uniformComboPrice:N0} VNĐ");
            Console.WriteLine($"═══════════════════════════════════════");
            Console.WriteLine($"");

            // Lưu từng combo với giá đồng nhất
            for (int i = 0; i < topRecommendations.Count; i++)
            {
                var rec = topRecommendations[i];

                // ✅ TẤT CẢ COMBO DÙNG CÙNG GIÁ
                decimal comboPrice = uniformComboPrice;
                decimal discountPercent = Math.Round((1 - comboPrice / rec.OriginalPrice) * 100, 2);

                // Tạo tên combo
                string comboName = GenerateComboName(rec.MenuItemNames);

                Console.WriteLine($"💾 Lưu combo {i + 1}: {comboName}");
                Console.WriteLine($"   📦 Gồm: {string.Join(", ", rec.MenuItemNames)}");
                Console.WriteLine($"   💵 Giá gốc: {rec.OriginalPrice:N0} VNĐ");
                Console.WriteLine($"   🏷️  Giá bán: {comboPrice:N0} VNĐ");
                Console.WriteLine($"   📉 Giảm: {discountPercent:N1}%");
                Console.WriteLine($"");

                // Lưu vào database
                var combo = await _comboService.SaveCompleteComboAsync(
                    comboName,
                    timeSlot.DayOfWeek,
                    timeSlot.HourOfDay,
                    timeSlot.HourOfDay + 2,
                    discountPercent,
                    comboPrice,
                    rec.OriginalPrice,
                    rec.MenuItemIds
                );

                // Set thông tin hiển thị
                combo.DayName = timeSlot.DayName;
                savedCombos.Add(combo);
            }

            return savedCombos;
        }

        /// <summary>
        /// Tạo tên combo thông minh
        /// </summary>
        private string GenerateComboName(List<string> itemNames)
        {
            if (itemNames.Count <= 2)
            {
                return $"Combo {string.Join(" + ", itemNames)}";
            }
            else if (itemNames.Count == 3)
            {
                return $"Combo {string.Join(", ", itemNames.Take(2))} & {itemNames.Last()}";
            }
            else
            {
                return $"Combo {string.Join(", ", itemNames.Take(2))} & {itemNames.Count - 2} món khác";
            }
        }

        /// <summary>
        /// Hiển thị combo lên UI
        /// </summary>
        private void DisplayCombos(List<ComboPromotion> combos)
        {
            // Combo 1
            if (combos.Count > 0)
            {
                Combo1Name = combos[0].ComboName;
                Combo1Price = combos[0].FormattedPrice;
                Combo1ImageUrl = GetValidImagePath(combos[0].Items.FirstOrDefault()?.ImageUrl);
            }

            // Combo 2
            if (combos.Count > 1)
            {
                Combo2Name = combos[1].ComboName;
                Combo2Price = combos[1].FormattedPrice;
                Combo2ImageUrl = GetValidImagePath(combos[1].Items.FirstOrDefault()?.ImageUrl);
            }

            // Combo 3
            if (combos.Count > 2)
            {
                Combo3Name = combos[2].ComboName;
                Combo3Price = combos[2].FormattedPrice;
                Combo3ImageUrl = GetValidImagePath(combos[2].Items.FirstOrDefault()?.ImageUrl);
            }
        }

        /// <summary>
        /// Kiểm tra và trả về đường dẫn ảnh hợp lệ
        /// </summary>
        // Trong file SaleManagerApp.ViewModels.HomePageViewModel.cs

        private string GetValidImagePath(string imagePath)
        {
            // Nếu dữ liệu từ DB trống, trả về ảnh mặc định ngay lập tức
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return "pack://application:,,,/Assets/Images/default-combo.png";
            }

            try
            {
                // Nếu là ảnh do người dùng upload (nằm trong thư mục thực thi bin/Debug)
                if (imagePath.Contains("Images/MenuItems"))
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    // Thay thế dấu / thành \ để tránh lỗi đường dẫn trên Windows
                    string cleanPath = imagePath.Replace("/", "\\");
                    string fullPath = Path.Combine(baseDir, cleanPath);

                    return File.Exists(fullPath) ? fullPath : "pack://application:,,,/Assets/Images/default-combo.png";
                }

                // Nếu là ảnh tài nguyên mẫu trong project
                return $"pack://application:,,,/Assets/Images/{imagePath}";
            }
            catch
            {
                return "pack://application:,,,/Assets/Images/default-combo.png";
            }
        }

        /// <summary>
        /// Cập nhật text banner
        /// </summary>
        private void UpdateBanner(ComboPromotion combo)
        {
            string dayNameUpper = combo.DayName.ToUpper();

            // Chuyển sang tiếng Việt
            var dayMapping = new Dictionary<string, string>
            {
                { "MONDAY", "THỨ HAI" },
                { "TUESDAY", "THỨ BA" },
                { "WEDNESDAY", "THỨ TƯ" },
                { "THURSDAY", "THỨ NĂM" },
                { "FRIDAY", "THỨ SÁU" },
                { "SATURDAY", "THỨ BẢY" },
                { "SUNDAY", "CHỦ NHẬT" }
            };

            string dayVN = dayMapping.ContainsKey(dayNameUpper)
                ? dayMapping[dayNameUpper]
                : dayNameUpper;

            BannerText = $"{dayVN} VUI VẺ - {combo.FormattedPrice}";
        }

        /// <summary>
        /// Hiển thị combo mặc định khi không có dữ liệu
        /// </summary>
        private void DisplayDefaultCombos()
        {
            string defaultImage = "pack://application:,,,/Assets/Images/default-combo.png";

            Combo1Name = "Combo đang cập nhật";
            Combo1Price = "Đang phân tích...";
            Combo1ImageUrl = defaultImage;

            Combo2Name = "Combo đang cập nhật";
            Combo2Price = "Đang phân tích...";
            Combo2ImageUrl = defaultImage;

            Combo3Name = "Combo đang cập nhật";
            Combo3Price = "Đang phân tích...";
            Combo3ImageUrl = defaultImage;

            BannerText = "KHUYẾN MÃI ĐẶC BIỆT - ĐỢI PHÂN TÍCH";
        }

        /// <summary>
        /// Refresh combo (có thể gọi từ button)
        /// </summary>
        public async Task RefreshCombosAsync()
        {
            Console.WriteLine("🔄 Refresh combo...");
            await LoadCombosAsync();
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}