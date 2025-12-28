using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using SaleManagerApp.Model;
using SaleManagerApp.Services;
using SaleManagerApp.Helpers;

namespace SaleManagerApp.ViewModels
{
    public class UserPageViewModel : BaseViewModel
    {
        private readonly StaffManagementService _service = new StaffManagementService();
        private ObservableCollection<Staff> _allStaffList;
        private ObservableCollection<Staff> _staffList;

        public ObservableCollection<Staff> StaffList
        {
            get => _staffList;
            set
            {
                _staffList = value;
                OnPropertyChanged(nameof(StaffList));
            }
        }

        private string _selectedPosition = null;
        public string SelectedPosition
        {
            get => _selectedPosition;
            set
            {
                _selectedPosition = value;
                OnPropertyChanged(nameof(SelectedPosition));
                LoadStaffData();
            }
        }

        public UserPageViewModel()
        {
            _allStaffList = new ObservableCollection<Staff>();
            StaffList = new ObservableCollection<Staff>();
            LoadStaffData();
        }

        private void LoadStaffData()
        {
            var result = _service.GetStaffByPosition(_selectedPosition);

            _allStaffList.Clear();

            if (result.Success && result.StaffList != null)
            {
                foreach (var staff in result.StaffList)
                {
                    _allStaffList.Add(staff);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    ToastService.ShowError(result.ErrorMessage);
                }
            }

            StaffList = new ObservableCollection<Staff>(_allStaffList);
        }

        public void CheckIn(string employeeId)
        {
            var result = _service.CheckIn(employeeId);

            if (result.Success)
            {
                ToastService.Show($"{result.Message} - Ca {result.WorkShift}");
                LoadStaffData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        public void CheckOut(string employeeId)
        {
            var result = _service.CheckOut(employeeId);

            if (result.Success)
            {
                ToastService.Show($"{result.Message} - Làm {result.WorkedHours} giờ");
                LoadStaffData();
            }
            else
            {
                ToastService.ShowError(result.Message);
            }
        }

        public void SearchStaff(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                StaffList = new ObservableCollection<Staff>(_allStaffList);
                return;
            }

            var filtered = _allStaffList.Where(s =>
                s.fullName.ToLower().Contains(keyword.ToLower()) ||
                s.StaffId.ToLower().Contains(keyword.ToLower()) ||
                s.phone.Contains(keyword) ||
                s.email.ToLower().Contains(keyword.ToLower())
            ).ToList();

            StaffList = new ObservableCollection<Staff>(filtered);
        }

        public void ThemNhanVien()
        {
            var addStaffWindow = new SaleManagerApp.Views.AddStaffWindow(_selectedPosition);
            addStaffWindow.ShowDialog();
            LoadStaffData();
        }

        public void EditStaff(string employeeId)
        {
            var staff = _allStaffList.FirstOrDefault(s => s.StaffId == employeeId);
            if (staff == null)
            {
                ToastService.ShowError("Không tìm thấy nhân viên!");
                return;
            }

            var editWindow = new SaleManagerApp.Views.EditStaffWindow(staff);
            if (editWindow.ShowDialog() == true)
            {
                LoadStaffData();
            }
        }

        public void DeleteStaff(string employeeId)
        {
            var staff = _allStaffList.FirstOrDefault(s => s.StaffId == employeeId);
            if (staff == null)
            {
                ToastService.ShowError("Không tìm thấy nhân viên!");
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa nhân viên '{staff.fullName}' (Mã: {employeeId})?\n\n" +
                "Lưu ý: Dữ liệu chấm công và lương của nhân viên này cũng sẽ bị xóa!",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes)
                return;

            var deleteResult = _service.DeleteStaff(employeeId);

            if (deleteResult.Success)
            {
                ToastService.Show(deleteResult.SuccessMessage);
                LoadStaffData();
            }
            else
            {
                ToastService.ShowError(deleteResult.ErrorMessage);
            }
        }

        public void ResetMonthlyHours(string employeeId)
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn reset giờ làm tháng về 0?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result != MessageBoxResult.Yes)
                return;

            var resetResult = _service.ResetMonthlyHours(employeeId);

            if (resetResult.Success)
            {
                ToastService.Show(resetResult.Message);
                LoadStaffData();
            }
            else
            {
                ToastService.ShowError(resetResult.Message);
            }
        }

        public void AddNextDaySchedule(string employeeId)
        {
            var scheduleWindow = new SaleManagerApp.Views.AddScheduleWindow(employeeId);
            if (scheduleWindow.ShowDialog() == true)
            {
                var selectedShift = scheduleWindow.SelectedShift;
                var result = _service.AddNextDaySchedule(employeeId, selectedShift);

                if (result.Success)
                {
                    ToastService.Show(result.Message);
                    LoadStaffData();
                }
                else
                {
                    ToastService.ShowError(result.Message);
                }
            }
        }
    }
}