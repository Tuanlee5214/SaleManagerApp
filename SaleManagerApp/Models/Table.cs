using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.Models
{
    public class Table : INotifyPropertyChanged
    {
        public string tableId { get; set; }
        public string tableName { get; set; }
        public string location { get; set; }
        public int seatCount { get; set; }

        private string _tableStatus;
        public string tableStatus
        {
            get => _tableStatus;
            set
            {
                _tableStatus = value;
                OnPropertyChanged(nameof(tableStatus));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

