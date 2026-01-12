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

        // =========================
        // DATA
        // =========================
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
                ApplyFilter();
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
                ApplyFilter();
            }
        }

        private string _selectedActionType;
        public string SelectedActionType
        {
            get => _selectedActionType;
            set
            {
                _selectedActionType = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public string[] ActionTypes =>
            new[] { "Tất cả", "IMPORT", "EXPORT", "UPDATE", "DELETE" };

        // =========================
        // COMMANDS
        // =========================
        public ICommand LoadCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand CloseCommand { get; }

        // =========================
        // ACTIONS
        // =========================
        public Action CloseAction { get; set; }

        public WarehouseLogsViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadLogs());
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

            try
            {
                var logs = _service.GetWarehouseLogs(300);

                foreach (var log in logs.OrderByDescending(l => l.CreatedAt))
                {
                    Logs.Add(log);
                }

                ApplyFilter();
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Không thể tải warehouse log: {ex.Message}");
            }
        }

        // =========================
        // FILTER
        // =========================
        private void ApplyFilter()
        {
            FilteredLogs.Clear();

            var filtered = Logs.AsEnumerable();

            // Search theo tên nguyên liệu hoặc historyId
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(l =>
                    (!string.IsNullOrEmpty(l.IngredientName) &&
                     l.IngredientName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    ||
                    (!string.IsNullOrEmpty(l.HistoryId) &&
                     l.HistoryId.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }

            // Filter theo ngày
            if (FromDate.HasValue)
            {
                filtered = filtered.Where(l => l.CreatedAt.Date >= FromDate.Value.Date);
            }

            if (ToDate.HasValue)
            {
                filtered = filtered.Where(l => l.CreatedAt.Date <= ToDate.Value.Date);
            }

            // Filter theo loại action
            if (!string.IsNullOrEmpty(SelectedActionType) &&
                SelectedActionType != "Tất cả")
            {
                filtered = filtered.Where(l => l.ActionType == SelectedActionType);
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
            SelectedActionType = "Tất cả";

            ApplyFilter();
        }
    }
}
