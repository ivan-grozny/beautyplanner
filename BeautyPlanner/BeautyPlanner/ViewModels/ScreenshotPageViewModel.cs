using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BeautyPlanner.Interfaces;
using BeautyPlanner.Models;
using Prism.Navigation;

namespace BeautyPlanner.ViewModels
{
    public class ScreenshotPageViewModel: ViewModelBase
    {
        private readonly IDatabaseService _databaseService;

        public ScreenshotPageViewModel(INavigationService navigationService, IDatabaseService databaseService) : base(navigationService)
        {
            _databaseService = databaseService;
        }

        public ObservableCollection<Day> Days { get; set; } = new ObservableCollection<Day>();

        #region MonthName property

        private string _monthName;
        public string MonthName
        {
            get => _monthName;
            set => SetProperty(ref _monthName, value);
        }

        #endregion

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            var date = parameters.GetValue<DateTime>("date");
            await LoadMonthAsync(date);
        }

        private async Task LoadMonthAsync(DateTime date)
        {
            Days.Clear();

            var month = await _databaseService.GetCurrentMonthAsync(date);

            MonthName = month.Name;
            var dayList = await _databaseService.GetDaysForMonthAsync(month.Id);
            var appointmentList = await _databaseService.GetAppointmentsForMonthAsync(month.Id);

            foreach (var day in dayList)
            {
                day.Appointments = new ObservableCollection<Appointment>(appointmentList.Where(appointment => appointment.DayId == day.Id));
                Days.Add(day);
            }
        }
    }
}
