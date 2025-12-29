using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class WarehouseLogViewModel : BaseViewModel
    {
        // ❌ new() → ✅ new WarehouseService()
        private readonly WarehouseService _service = new WarehouseService();

        // ===== LOGS =====
        public ObservableCollection<WarehouseLog> Logs { get; }
            = new ObservableCollection<WarehouseLog>();

        // ===== FILTER =====
        private string _filterType = "Tất cả";
        public string FilterType
        {
            get => _filterType;
            set
            {
                _filterType = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        // ❌ new() → ✅ new ObservableCollection<WarehouseLog>()
        private ObservableCollection<WarehouseLog> _allLogs
            = new ObservableCollection<WarehouseLog>();

        // ===== STATS =====
        public int TotalImports => _allLogs.Count(l => l.ActionType == "IMPORT");
        public int TotalExports => _allLogs.Count(l => l.ActionType == "EXPORT");
        public int TotalUpdates => _allLogs.Count(l => l.ActionType == "UPDATE");
        public int TotalDeletes => _allLogs.Count(l => l.ActionType == "DELETE");

        // ===== COMMANDS =====
        public ICommand LoadCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CloseCommand { get; }

        // ===== ACTIONS =====
        public Action CloseAction { get; set; }

        public WarehouseLogViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadData());
            RefreshCommand = new RelayCommand(_ => LoadData());
            CloseCommand = new RelayCommand(_ => CloseAction?.Invoke());

            LoadData();
        }

        // ===== LOAD DATA =====
        private void LoadData()
        {
            try
            {
                var logs = _service.GetWarehouseLogs(200);

                _allLogs.Clear();
                foreach (var log in logs)
                    _allLogs.Add(log);

                ApplyFilter();
                RefreshStats();
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Không thể tải log: {ex.Message}");
            }
        }

        // ===== FILTER =====
        private void ApplyFilter()
        {
            Logs.Clear();

            var filtered = FilterType == "Tất cả"
                ? _allLogs
                : _allLogs.Where(l => GetDisplayType(l.ActionType) == FilterType);

            foreach (var log in filtered)
                Logs.Add(log);
        }

        // ❌ switch expression → ✅ switch thường (C# 7.3)
        private string GetDisplayType(string actionType)
        {
            switch (actionType)
            {
                case "IMPORT":
                    return "Nhập kho";
                case "EXPORT":
                    return "Xuất kho";
                case "UPDATE":
                    return "Cập nhật";
                case "DELETE":
                    return "Xóa";
                default:
                    return "Khác";
            }
        }

        // ===== STATS =====
        private void RefreshStats()
        {
            OnPropertyChanged(nameof(TotalImports));
            OnPropertyChanged(nameof(TotalExports));
            OnPropertyChanged(nameof(TotalUpdates));
            OnPropertyChanged(nameof(TotalDeletes));
        }
    }
}
