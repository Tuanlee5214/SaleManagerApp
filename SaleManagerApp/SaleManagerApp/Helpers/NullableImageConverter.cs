using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SaleManagerApp.Helpers
{
    /// <summary>
    /// Converter xử lý ImagePath null hoặc empty string
    /// </summary>
    public class NullableImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Kiểm tra null hoặc chuỗi rỗng
            if (value == null || (value is string s && string.IsNullOrWhiteSpace(s)))
            {
                // TRẢ VỀ DependencyProperty.UnsetValue THAY VÌ NULL
                return System.Windows.DependencyProperty.UnsetValue;
            }

            try
            {
                string path = value.ToString();

                // Kiểm tra file có tồn tại không
                if (!File.Exists(path))
                {
                    System.Diagnostics.Debug.WriteLine($"[ImageConverter] File not found: {path}");
                    return System.Windows.DependencyProperty.UnsetValue;
                }

                // Tạo BitmapImage từ đường dẫn
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache; // Tránh cache cũ
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.DecodePixelWidth = 200; // Tăng lên để ảnh rõ hơn
                bitmap.EndInit();
                bitmap.Freeze(); // Quan trọng: Freeze để dùng được cross-thread

                System.Diagnostics.Debug.WriteLine($"[ImageConverter] Loaded successfully: {path}");
                return bitmap;
            }
            catch (Exception ex)
            {
                // Nếu lỗi load ảnh, trả về UnsetValue
                System.Diagnostics.Debug.WriteLine($"[ImageConverter] Error loading image: {ex.Message}");
                return System.Windows.DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}