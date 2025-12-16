using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SaleManagerApp.Helpers
{
    public class QuantityToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int quantity = (int)value;
            string target = parameter?.ToString();

            if (target == "AddButton")
                return quantity == 0 ? Visibility.Visible : Visibility.Collapsed;

            if (target == "Panel")
                return quantity > 0 ? Visibility.Visible : Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
