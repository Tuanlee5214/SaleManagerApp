using System.Windows;

namespace SaleManagerApp.Views
{
    public partial class AddScheduleWindow : Window
    {
        public string SelectedShift { get; private set; }

        public AddScheduleWindow(string employeeId)
        {
            InitializeComponent();
        }

        private void MorningShift_Click(object sender, RoutedEventArgs e)
        {
            SelectedShift = "Sáng";
            this.DialogResult = true;
        }

        private void AfternoonShift_Click(object sender, RoutedEventArgs e)
        {
            SelectedShift = "Chiều";
            this.DialogResult = true;
        }

        private void EveningShift_Click(object sender, RoutedEventArgs e)
        {
            SelectedShift = "Tối";
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}