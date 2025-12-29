using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SaleManagerApp.Helpers
{
    /// <summary>
    /// Chuyển null → Collapsed, not null → Visible
    /// Dùng để ẩn/hiện element khi giá trị null
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Nếu null → ẩn
            if (value == null)
                return Visibility.Collapsed;

            // Nếu là string rỗng → ẩn
            if (value is string str && string.IsNullOrWhiteSpace(str))
                return Visibility.Collapsed;

            // Có giá trị → hiện
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}