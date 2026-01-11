using SaleManagerApp.Models;
using SaleManagerApp.Services;
using SaleManagerApp.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class WarehouseLogsViewModel : BaseViewModel
    {
        private readonly WarehouseService _service = new WarehouseService();

        public ObservableCollection<WarehouseLog> Logs { get; } = new ObservableCollection<WarehouseLog>();
        public ObservableCollection<WarehouseLog> FilteredLogs { get; } = new ObservableCollection<WarehouseLog>();

        public Array LogTypes => Enum.GetValues(typeof(LogType));

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private LogType? _selectedLogType;
        public LogType? SelectedLogType
        {
            get => _selectedLogType;
            set { _selectedLogType = value; OnPropertyChanged(); ApplyFilter(); }
        }

        public ICommand LoadCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand CloseCommand { get; }

        public Action CloseAction { get; set; }

        public WarehouseLogsViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadLogs());
            ClearFilterCommand = new RelayCommand(_ => ClearFilter());
            CloseCommand = new RelayCommand(_ => CloseAction?.Invoke());
            LoadLogs();
        }

        private void LoadLogs()
        {
            Logs.Clear();
            var result = _service.GetWarehouseLogs();

            

            foreach (var log in result)
                Logs.Add(log);

            ApplyFilter();  
        }

        private void ApplyFilter()
        {
            FilteredLogs.Clear();
            var data = Logs.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                data = data.Where(x =>
                    !string.IsNullOrEmpty(x.IngredientName) &&
                    x.IngredientName
                        .IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0
                );
            }

            if (SelectedLogType.HasValue)
                data = data.Where(x => x.Type == SelectedLogType.Value);

            foreach (var item in data)
                FilteredLogs.Add(item);
        }

        private void ClearFilter()
        {
            SearchText = string.Empty;
            SelectedLogType = null;
            LoadLogs();
        }
    }
}
