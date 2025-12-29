using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using SaleManagerApp.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class WarehousePageViewModel : BaseViewModel
    {
        // ❌ new() → ✅ new WarehouseService()
        private readonly WarehouseService _service = new WarehouseService();

        // ===== DATA =====
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

        // ===== FILTER =====
        private string _selectedFilter = null;  // null = "Tất cả"
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();
                LoadData();
            }
        }

        // ===== COMMANDS =====
        public ICommand LoadCommand { get; }
        public ICommand OpenCreateIngredientCommand { get; }
        public ICommand OpenImportCommand { get; }
        public ICommand OpenBatchDetailCommand { get; }
        public ICommand OpenWarehouseLogCommand { get; }

        public WarehousePageViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadData());
            OpenCreateIngredientCommand = new RelayCommand(_ => OpenCreateIngredient());
            OpenImportCommand = new RelayCommand(_ => OpenImport());
            OpenBatchDetailCommand = new RelayCommand(
                _ => OpenBatchDetail(),
                _ => SelectedIngredient != null
            );
            OpenWarehouseLogCommand = new RelayCommand(_ => OpenWarehouseLog());

            LoadData();
        }

        // ===== LOAD DATA =====
        private void LoadData()
        {
            Ingredients.Clear();

            try
            {
                var data = _service.GetAllIngredients(_selectedFilter);
                foreach (var item in data)
                    Ingredients.Add(item);
            }
            catch
            {
                ToastService.ShowError("Không thể tải dữ liệu kho");
            }
        }

        // ===== CREATE INGREDIENT =====
        private void OpenCreateIngredient()
        {
            var vm = new CreateIngredientViewModel();
            var window = new CreateIngredientView();

            window.DataContext = vm;

            vm.ReloadAction = LoadData;
            vm.CloseAction = window.Close;

            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        // ===== IMPORT =====
        private void OpenImport()
        {
            var vm = new ImportIngredientViewModel();
            var window = new ImportIngredientView();  // Constructor mặc định
            window.DataContext = vm;

            vm.ReloadAction = LoadData;
            vm.CloseAction = window.Close;

            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        // ===== BATCH DETAIL =====
        private void OpenBatchDetail()
        {
            if (SelectedIngredient == null) return;

            var vm = new BatchDetailViewModel();
            var window = new BatchDetailView();

            window.DataContext = vm;

            vm.LoadData(SelectedIngredient.IngredientId);
            vm.ReloadParentAction = LoadData;
            vm.CloseAction = window.Close;

            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        // ===== WAREHOUSE LOG =====
        private void OpenWarehouseLog()
        {
            var vm = new WarehouseLogViewModel();
            var window = new WarehouseLogView();

            window.DataContext = vm;
            vm.CloseAction = window.Close;

            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }
    }
}
