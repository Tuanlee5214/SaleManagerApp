using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SaleManagerApp.Services;
using SaleManagerApp.Navigation;
using SaleManagerApp.Helpers;

namespace SaleManagerApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserService _userService = new UserService();
        private readonly StaffManagementService _staffService = new StaffManagementService();

        public string Username { get; set; }

        private string _errorMessage;
        private string _successMessage;

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set { _successMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            //LoginCommand = new RelayCommand(async (obj) => await LoginAsync(obj));
        }

        private bool _overlayVisible;
        public bool OverlayVisible
        {
            get => _overlayVisible;
            set { _overlayVisible = value; OnPropertyChanged(); }
        }

        public async Task LoginAsync(string Password)
        {
            OverlayVisible = true;
            ErrorMessage = " ";
            SuccessMessage = " ";

            var result = await Task.Run(() =>
            {
                return _userService.Login(Username, Password);
            });

            OverlayVisible = false;

            if (!result.Success)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ToastService.ShowErrorLogin(result.ErrorMessage);
                });
                return;
            }

            // SET USER VÀO SESSION
            UserSession.SetUser(result.user);

            // LOAD EMPLOYEE INFO (nếu user có liên kết với employee)
            if (!string.IsNullOrEmpty(result.user.staffId))
            {
                var employee = await Task.Run(() => _staffService.GetEmployeeByUserId(result.user.userId));
                if (employee != null)
                {
                    UserSession.SetEmployee(employee);
                    System.Diagnostics.Debug.WriteLine($"[Login] Employee loaded: {employee.fullName}, Position: {employee.position}");
                }
            }

            // DEBUG: In ra quyền của user
            System.Diagnostics.Debug.WriteLine("===== LOGIN INFO =====");
            System.Diagnostics.Debug.WriteLine($"User: {result.user.userName}");
            System.Diagnostics.Debug.WriteLine($"Group ID: {result.user.groupId}");
            System.Diagnostics.Debug.WriteLine($"Is Admin: {UserSession.IsAdmin()}");
            System.Diagnostics.Debug.WriteLine($"Is Manager: {UserSession.IsManager()}");
            System.Diagnostics.Debug.WriteLine($"Can Manage Attendance: {UserSession.CanManageAttendance()}");
            System.Diagnostics.Debug.WriteLine("======================");

            SuccessMessage = result.SuccesMessage;
            NavigationService.Navigate(new MainLayoutViewModel());
        }
    }
}