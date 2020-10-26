using System;
using Prism.Mvvm;
using SQLite;

namespace BeautyPlanner.Models
{
    public class Appointment : BindableBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private TimeSpan _time;
        public TimeSpan Time 
        {
            get { return _time; }
            set { SetProperty(ref _time, value); }
        }

        public string LinkOrText { get; set; }

        public int DayId { get; set; }

        public int MonthId { get; set; }

        private string _appointmentType = Constants.Manicure;
        public string AppointmentType {

            get { return _appointmentType; }
            set
            {
                SetProperty(ref _appointmentType, value); 
                RaisePropertyChanged(nameof(Time)); //it is required for the AppointmentLabelBehavior to work
            }
               
        }

        public bool IsFreeDay { get; set; }
    }
}
