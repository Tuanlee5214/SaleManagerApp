
using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class BatchDetailViewModel : BaseViewModel
    {
        // ❗ FIX C# 7.3
        private readonly WarehouseService _service = new WarehouseService();

        // ===== INGREDIENT INFO =====
        private IngredientItem _ingredient;
        public IngredientItem Ingredient
        {
            get => _ingredient;
            set
            {
                _ingredient = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalQuantity));
                OnPropertyChanged(nameof(HasExpiredBatch));
            }
        }

        public int TotalQuantity => Ingredient?.TotalQuantity ?? 0;
        public bool HasExpiredBatch => Ingredient?.HasExpiredBatch ?? false;

        // ===== BATCH LIST =====
        public ObservableCollection<IngredientBatchHistory> Batches { get; }
            = new ObservableCollection<IngredientBatchHistory>();

        private IngredientBatchHistory _selectedBatch;
        public IngredientBatchHistory SelectedBatch
        {
            get => _selectedBatch;
            set
            {
                _selectedBatch = value;
                OnPropertyChanged();

                (DeleteExpiredBatchCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (UpdateBatchCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // ===== EXPORT INPUT =====
        private int _exportQuantity;
        public int ExportQuantity
        {
            get => _exportQuantity;
            set
            {
                _exportQuantity = value;
                OnPropertyChanged();
            }
        }

        // ===== UPDATE BATCH INPUT =====
        private int _addQuantity;
        public int AddQuantity
        {
            get => _addQuantity;
            set
            {
                _addQuantity = value;
                OnPropertyChanged();
            }
        }

        private DateTime _newExpiryDate = DateTime.Today.AddDays(7);
        public DateTime NewExpiryDate
        {
            get => _newExpiryDate;
            set
            {
                _newExpiryDate = value;
                OnPropertyChanged();
            }
        }

        // ===== COMMANDS =====
        public ICommand ExportCommand { get; }
        public ICommand UpdateBatchCommand { get; }
        public ICommand DeleteExpiredBatchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CloseCommand { get; }

        // ===== ACTIONS =====
        public Action CloseAction { get; set; }
        public Action ReloadParentAction { get; set; }

        public BatchDetailViewModel()
        {
            ExportCommand = new RelayCommand(_ => Export());
            UpdateBatchCommand = new RelayCommand(_ => UpdateBatch(), _ => SelectedBatch != null);
            DeleteExpiredBatchCommand = new RelayCommand(_ => DeleteExpiredBatch(), _ => SelectedBatch != null);
            RefreshCommand = new RelayCommand(_ => Refresh());
            CloseCommand = new RelayCommand(_ => CloseAction?.Invoke());
        }

        // ===== LOAD DATA =====
        public void LoadData(string ingredientId)
        {
            try
            {
                Ingredient = _service.GetIngredientById(ingredientId);

                Batches.Clear();
                foreach (var batch in Ingredient.Histories)
                    Batches.Add(batch);
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Không thể tải dữ liệu: {ex.Message}");
            }
        }

        private void Refresh()
        {
            if (Ingredient == null) return;
            LoadData(Ingredient.IngredientId);
        }

        // ===== EXPORT =====
        private void Export()
        {
            if (ExportQuantity <= 0)
            {
                ToastService.ShowError("Số lượng xuất phải > 0");
                return;
            }

            if (HasExpiredBatch)
            {
                MessageBox.Show(
                    "Không thể xuất kho vì có batch hết hạn chưa xử lý!\nVui lòng xóa batch hết hạn trước.",
                    "Cảnh báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (ExportQuantity > TotalQuantity)
            {
                ToastService.ShowError($"Không đủ tồn kho! Chỉ còn {TotalQuantity} {Ingredient.Unit}");
                return;
            }

            try
            {
                string employeeId = "EM00001";

                _service.ExportIngredient(
                    Ingredient.IngredientId,
                    ExportQuantity,
                    employeeId,
                    $"Xuất {ExportQuantity} {Ingredient.Unit}"
                );

                ToastService.Show($"Xuất kho thành công {ExportQuantity} {Ingredient.Unit}");

                ExportQuantity = 0;
                Refresh();
                ReloadParentAction?.Invoke();
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Xuất kho thất bại: {ex.Message}");
            }
        }

        // ===== UPDATE BATCH =====
        private void UpdateBatch()
        {
            if (AddQuantity <= 0)
            {
                ToastService.ShowError("Số lượng thêm phải > 0");
                return;
            }

            if (NewExpiryDate < DateTime.Today)
            {
                ToastService.ShowError("Ngày hết hạn không hợp lệ");
                return;
            }

            try
            {
                _service.UpdateBatch(
                    SelectedBatch.HistoryId,
                    AddQuantity,
                    NewExpiryDate,
                    $"Cập nhật thêm {AddQuantity} {Ingredient.Unit}"
                );

                ToastService.Show("Cập nhật batch thành công");

                AddQuantity = 0;
                Refresh();
                ReloadParentAction?.Invoke();
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Cập nhật batch thất bại: {ex.Message}");
            }
        }

        // ===== DELETE EXPIRED =====
        private void DeleteExpiredBatch()
        {
            if (!SelectedBatch.IsExpired)
            {
                ToastService.ShowError("Chỉ có thể xóa batch đã hết hạn");
                return;
            }

            var result = MessageBox.Show(
                $"Xác nhận xóa batch hết hạn?\n\n" +
                $"Nguyên liệu: {Ingredient.IngredientName}\n" +
                $"Số lượng: {SelectedBatch.RemainingQuantity} {Ingredient.Unit}\n" +
                $"Ngày hết hạn: {SelectedBatch.ExpiryDate:dd/MM/yyyy}",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _service.DeleteExpiredBatch(
                    SelectedBatch.HistoryId,
                    "Xóa batch hết hạn"
                );

                ToastService.Show("Đã xóa batch hết hạn");

                Refresh();
                ReloadParentAction?.Invoke();
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Xóa batch thất bại: {ex.Message}");
            }
        }
    }
}
