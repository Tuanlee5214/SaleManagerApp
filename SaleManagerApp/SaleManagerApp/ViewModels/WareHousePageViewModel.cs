using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using SaleManagerApp.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace SaleManagerApp.ViewModels
{
    public class WarehousePageViewModel : BaseViewModel
    {
        private readonly WarehouseService _service = new WarehouseService();

        public ObservableCollection<IngredientItem> Ingredients { get; }
            = new ObservableCollection<IngredientItem>();

        private IngredientItem _selectedIngredient;

        public IngredientItem SelectedIngredient
        {
            get => _selectedIngredient;
            set
            {
                _selectedIngredient = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand OpenImportCommand { get; }
        public ICommand OpenExportCommand { get; }
        public ICommand OpenCreateIngredientCommand { get; } // ✅ MỚI

        public WarehousePageViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadData());
            OpenImportCommand = new RelayCommand(_ => OpenImport(), _ => SelectedIngredient != null);
            OpenExportCommand = new RelayCommand(_ => OpenExport(), _ => SelectedIngredient != null);
            OpenCreateIngredientCommand = new RelayCommand(_ => OpenCreateIngredient()); // ✅
        }

        public void LoadData()
        {
            Ingredients.Clear();

            var result = _service.GetAllIngredients();

            if (!result.Success)
            {
                ToastService.ShowError(result.ErrorMessage);
                return;
            }

            foreach (var item in result.IngredientList)
            {
                Ingredients.Add(item);
            }
        }

        private void OpenImport()
        {
            if (SelectedIngredient == null)
                return;

            var vm = new ImportIngredientViewModel();
            var window = new ImportIngredientView(vm);

            vm.SetIngredient(SelectedIngredient);
            vm.ReloadAction = LoadData;
            vm.CloseAction = () => window.Close();

            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void OpenExport()
        {
            if (SelectedIngredient == null) return;

            var vm = new ExportIngredientViewModel();
            var window = new ExportIngredientView(vm);

            vm.IngredientId = SelectedIngredient.IngredientId;
            vm.CurrentQuantity = SelectedIngredient.TotalQuantity;

            vm.ReloadAction = LoadData;
            vm.CloseAction = () => window.Close();

            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        // =========================
        // CREATE INGREDIENT
        // =========================
        private void OpenCreateIngredient()
        {
            var vm = new CreateIngredientViewModel();
            var window = new CreateIngredientView(vm);

            vm.ReloadAction = LoadData;
            vm.CloseAction = () => window.Close();

            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }
    }
}
