using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SaleManagerApp.Views
{
    public partial class ExportIngredientView : Window
    {
        public ExportIngredientView()
        {
            InitializeComponent();
        }

        private void NumberOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}