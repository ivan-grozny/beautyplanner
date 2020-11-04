using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using SQLite;

namespace BeautyPlanner.Models
{
    public class Day : BindableBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }

        [Ignore]
        public ObservableCollection<Appointment> Appointments { get; set; }

        public int MonthId { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
    }
}
