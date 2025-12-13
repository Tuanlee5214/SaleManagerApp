using SaleManagerApp.Models;
using SaleManagerApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SaleManagerApp.Helpers
{
    public class MenuItemToQuantityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return 0;

            var cartItems = values[0] as IEnumerable<CartItem>;
            var menuItem = values[1] as MenuItem;

            if (cartItems == null || menuItem == null)
                return 0;

            var cartItem = cartItems.FirstOrDefault(c => c.Item.menuItemId == menuItem.menuItemId);
            return cartItem?.Quantity ?? 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
