using System.Windows;
using SaleManagerApp.ViewModels;

namespace SaleManagerApp.Views
{
    public partial class ImportIngredientView : Window
    {
        public ImportIngredientView()
        {
            InitializeComponent();
        }

        public ImportIngredientView(ImportIngredientViewModel vm) : this()
        {
            DataContext = vm;
        }
    }
}