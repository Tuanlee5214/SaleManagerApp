using System;
using System.Globalization;
using System.Windows.Data;

namespace SaleManagerApp.Helpers
{
    /// <summary>
    /// So sánh giá trị binding với ConverterParameter
    /// Dùng cho RadioButton IsChecked
    /// </summary>
    public class FilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Cả 2 đều null → True
            if (value == null && parameter == null)
                return true;

            // Chỉ 1 trong 2 null → False
            if (value == null || parameter == null)
                return false;

            // So sánh giá trị
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Khi RadioButton được check → trả về ConverterParameter
            if (value is bool isChecked && isChecked && parameter != null)
            {
                return parameter.ToString();
            }

            return Binding.DoNothing;
        }
    }
}