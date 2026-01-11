using SaleManagerApp.Helpers;
using SaleManagerApp.Services;
using System;
using System.Windows.Input;

namespace SaleManagerApp.ViewModels
{
    public class UpdateBatchViewModel : BaseViewModel
    {
        private readonly WarehouseService _service = new WarehouseService();

        private string _batchId;
        public string BatchId
        {
            get => _batchId;
            set
            {
                _batchId = value;
                OnPropertyChanged();
            }
        }

        private int _currentUpdateNumber;
        public int CurrentUpdateNumber
        {
            get => _currentUpdateNumber;
            set
            {
                _currentUpdateNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NextUpdateNumber));
            }
        }

        public int NextUpdateNumber => CurrentUpdateNumber + 1;

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        private DateTime _importDate = DateTime.Now;
        public DateTime ImportDate
        {
            get => _importDate;
            set
            {
                _importDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _expiryDate;
        public DateTime? ExpiryDate
        {
            get => _expiryDate;
            set
            {
                _expiryDate = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }
        public Action ReloadAction { get; set; }

        public UpdateBatchViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(BatchId))
            {
                ToastService.ShowError("BatchId không hợp lệ");
                return;
            }

            if (Quantity <= 0)
            {
                ToastService.ShowError("Số lượng phải lớn hơn 0");
                return;
            }

             _service.UpdateBatch(
                BatchId,
                Quantity,
                ImportDate,
                ExpiryDate,
                "System"
            );


        }
    }
}