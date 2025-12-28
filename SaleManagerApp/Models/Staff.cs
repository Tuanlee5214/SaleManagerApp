using System;
using System.IO;

namespace SaleManagerApp.Model
{
    public class Staff
    {
        public string fullName { get; set; }
        public string dateofBirth { get; set; }
        public string position { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string groupId { get; set; }
        public string StaffId { get; set; }
        public string DateStart { get; set; }
        public string ImagePath { get; set; }
        public decimal TotalHoursOfMonth { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public string WorkShift { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImagePath))
                    return null;

                string path = ImagePath;
                if (!Path.HasExtension(path))
                    path += ".jpg";

                return Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    path.Replace("/", "\\")
                );
            }
        }

        public string AttendanceStatus
        {
            get
            {
                if (CheckInTime.HasValue && !string.IsNullOrEmpty(WorkShift))
                    return $"Đã chấm công vào - Ca {WorkShift}";
                return "Chưa chấm công";
            }
        }
    }
}