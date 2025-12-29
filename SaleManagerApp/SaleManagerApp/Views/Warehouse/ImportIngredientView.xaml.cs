using SaleManagerApp.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SaleManagerApp.Views
{
    public partial class ImportIngredientView : Window
    {
        public ImportIngredientView(ImportIngredientViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        // Chỉ cho nhập số
        private void NumberOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
    }
}
