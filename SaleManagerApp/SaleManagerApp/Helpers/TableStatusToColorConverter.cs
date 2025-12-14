using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SaleManagerApp.Helpers
{
    public class TableStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value?.ToString();
            return status == "Còn trống" ? Brushes.LightGreen :
                   status == "Đã có khách" ? Brushes.OrangeRed :
                   Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
