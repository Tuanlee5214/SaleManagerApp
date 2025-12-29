using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class WarehouseLogsViewModel : BaseViewModel
    {
        private readonly WarehouseService _service = new WarehouseService();

        public ObservableCollection<WarehouseLog> Logs { get; }
            = new ObservableCollection<WarehouseLog>();

        public ObservableCollection<WarehouseLog> FilteredLogs { get; }
            = new ObservableCollection<WarehouseLog>();

        // =========================
        // FILTER OPTIONS
        // =========================
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                _fromDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                _toDate = value;
                OnPropertyChanged();
            }
        }

        private LogType? _selectedLogType;
        public LogType? SelectedLogType
        {
            get => _selectedLogType;
            set
            {
                _selectedLogType = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public Array LogTypes => Enum.GetValues(typeof(LogType));

        // =========================
        // COMMANDS
        // =========================
        public ICommand LoadCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand CloseCommand { get; }

        // =========================
        // ACTIONS
        // =========================
        public Action CloseAction { get; set; }

        public WarehouseLogsViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadLogs());
            SearchCommand = new RelayCommand(_ => LoadLogs());
            ClearFilterCommand = new RelayCommand(_ => ClearFilter());
            CloseCommand = new RelayCommand(_ => CloseAction?.Invoke());

            LoadLogs();
        }

        // =========================
        // LOAD LOGS
        // =========================
        private void LoadLogs()
        {
            Logs.Clear();

            var result = _service.GetWarehouseLogs(
                ingredientId: null,
                fromDate: FromDate,
                toDate: ToDate
            );

            if (!result.Success)
            {
                ToastService.ShowError(result.ErrorMessage);
                return;
            }

            foreach (var log in result.Logs.OrderByDescending(l => l.ActionDate))
            {
                Logs.Add(log);
            }

            ApplyFilter();
        }

        // =========================
        // FILTER
        // =========================
        private void ApplyFilter()
        {
            FilteredLogs.Clear();

            var filtered = Logs.AsEnumerable();

            // Filter by search text
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(l =>
                    l.IngredientName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    l.BatchId.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by log type
            if (SelectedLogType.HasValue)
            {
                filtered = filtered.Where(l => l.Type == SelectedLogType.Value);
            }

            foreach (var log in filtered)
            {
                FilteredLogs.Add(log);
            }
        }

        // =========================
        // CLEAR FILTER
        // =========================
        private void ClearFilter()
        {
            SearchText = string.Empty;
            FromDate = null;
            ToDate = null;
            SelectedLogType = null;
            LoadLogs();
        }
    }
}