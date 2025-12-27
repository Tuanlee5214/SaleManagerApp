using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class WarehousePageViewModel : BaseViewModel
    {
        private readonly WarehouseService _service = new WarehouseService();

        public ObservableCollection<Ingredient> Ingredients { get; }
            = new ObservableCollection<Ingredient>();

        private Ingredient _selectedIngredient;
        public Ingredient SelectedIngredient
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

        public WarehousePageViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadData());
            OpenImportCommand = new RelayCommand(_ => OpenImport(), _ => SelectedIngredient != null);
            OpenExportCommand = new RelayCommand(_ => OpenExport(), _ => SelectedIngredient != null);
        }

        public void LoadData()
        {
            Ingredients.Clear();
            var list = _service.GetAllIngredients();
            foreach (var item in list)
                Ingredients.Add(item);
        }

        private void OpenImport()
        {
            // UI mở popup ImportWarehouseView
        }

        private void OpenExport()
        {
            // UI mở popup ExportWarehouseView
        }
    }
}
