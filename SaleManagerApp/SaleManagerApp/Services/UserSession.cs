using SaleManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.Services
{
    public class UserSession
    {
        public static User CurrentUser { get; private set;}
        public static DateTime LoginTime { get; private set; }

        public static void SetUser(User user)
        {
            CurrentUser = user;
            LoginTime = DateTime.Now;
        }

        public static bool IsExpired()
        {
            return (DateTime.Now - LoginTime).TotalMinutes > 30;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }
    }

}
