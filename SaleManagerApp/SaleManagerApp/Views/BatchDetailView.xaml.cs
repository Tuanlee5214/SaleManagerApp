using SaleManagerApp.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SaleManagerApp.Views
{
    public partial class BatchDetailView : Window
    {
        public BatchDetailView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is BatchDetailViewModel vm)
            {
                vm.PropertyChanged += ViewModel_PropertyChanged;
                UpdateWarningVisibility(vm.HasExpiredBatch);
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BatchDetailViewModel.HasExpiredBatch))
            {
                var vm = sender as BatchDetailViewModel;
                UpdateWarningVisibility(vm?.HasExpiredBatch ?? false);
            }
        }

        private void UpdateWarningVisibility(bool hasExpired)
        {
            // Tìm Border có tên WarningBanner và set Visibility
            var warning = this.FindName("WarningBanner") as Border;
            if (warning != null)
            {
                warning.Visibility = hasExpired ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}