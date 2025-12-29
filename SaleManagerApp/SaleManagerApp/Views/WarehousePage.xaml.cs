using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SaleManagerApp.ViewModels;

namespace SaleManagerApp.Views
{
    public partial class WarehousePage : UserControl
    {
        public WarehousePage()
        {
            InitializeComponent();
            DataContext = new WarehousePageViewModel();
        }

        private void FilterButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && DataContext is WarehousePageViewModel vm)
            {
                string filterValue = rb.Tag?.ToString();
                vm.SelectedFilter = string.IsNullOrEmpty(filterValue) ? null : filterValue;
            }
        }

        private void IngredientCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string ingredientId)
            {
                if (DataContext is WarehousePageViewModel vm)
                {
                    var ingredient = vm.Ingredients.FirstOrDefault(i => i.IngredientId == ingredientId);
                    if (ingredient != null)
                    {
                        vm.SelectedIngredient = ingredient;
                        vm.OpenBatchDetailCommand.Execute(null);
                    }
                }
            }
        }
    }
}