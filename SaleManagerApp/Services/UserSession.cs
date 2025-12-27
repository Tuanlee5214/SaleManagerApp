using SaleManagerApp.Models;
using SaleManagerApp.Model;
using System;

namespace SaleManagerApp.Services
{
    public class UserSession
    {
        public static User CurrentUser { get; private set; }
        public static Staff CurrentEmployee { get; private set; }
        public static DateTime LoginTime { get; private set; }
        public static bool JustLoggedIn { get; set; }

        public static void SetUser(User user)
        {
            CurrentUser = user;
            JustLoggedIn = true;
            LoginTime = DateTime.Now;
        }

        // SET EMPLOYEE (gọi sau khi load từ DB)
        public static void SetEmployee(Staff employee)
        {
            CurrentEmployee = employee;
        }

        public static bool IsExpired()
        {
            return (DateTime.Now - LoginTime).TotalMinutes > 30;
        }

        // KIỂM TRA CÓ PHẢI ADMIN KHÔNG
        public static bool IsAdmin()
        {
            if (CurrentUser == null) return false;
            return CurrentUser.groupId == "GR00001"; // Admin group ID
        }

        // KIỂM TRA CÓ PHẢI QUẢN LÝ KHÔNG
        public static bool IsManager()
        {
            if (CurrentEmployee == null) return false;

            string position = CurrentEmployee.position?.ToLower() ?? "";
            return position.Contains("quản lý") ||
                   position.Contains("quan ly") ||
                   position.Contains("manager");
        }

        // KIỂM TRA CÓ QUYỀN CHẤM CÔNG KHÔNG
        public static bool CanManageAttendance()
        {
            return IsAdmin() || IsManager();
        }

        public static void Logout()
        {
            CurrentUser = null;
            CurrentEmployee = null;
        }
    }
}