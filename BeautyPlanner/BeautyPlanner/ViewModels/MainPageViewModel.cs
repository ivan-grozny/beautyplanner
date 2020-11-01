using System;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BeautyPlanner.Helpers;
using BeautyPlanner.Interfaces;
using BeautyPlanner.Models;
using Prism.Commands;
using Prism.Services.Dialogs;
using Xamarin.Essentials;

namespace BeautyPlanner.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly IDialogService _dialogService;
        private DateTime _currrentDateTime;

        public MainPageViewModel(INavigationService navigationService, IDatabaseService databaseService,
            IDialogService dialogService)
            : base(navigationService)
        {
            Title = "Свободные места";

            ShowTimetableCommand = new DelegateCommand(ShowTimetable);

            _databaseService = databaseService;
            _dialogService = dialogService;

            LoadMonthAsync(DateTime.Now).SafeFireAndForget(true, e => LoadMonthAsync(DateTime.Now));
            _currrentDateTime = DateTime.Now;

            AddAppointmentCommand = new DelegateCommand<Day>(AddAppointment);
            ViewAppointmentCommand = new DelegateCommand<Appointment>(ViewAppointment);

            LoadNextMonthCommand = new DelegateCommand(LoadNextMonth);
            LoadPreviousMonthCommand = new DelegateCommand(LoadPreviousMonth);
        }

       // public ObservableCollection<Month> Months { get; set; } = new ObservableCollection<Month>();

        #region ShowTimetableCommand

        public DelegateCommand ShowTimetableCommand { get; }

        private async void ShowTimetable()
        {
            var navParameters = new NavigationParameters();
            navParameters.Add("date", _currrentDateTime);
            await NavigationService.NavigateAsync("NavigationPage/ScreenshotPage", navParameters);
        }

        #endregion

        public ObservableCollection<Day> Days { get; set; } = new ObservableCollection<Day>();

        #region MonthName property

        private string _monthName;
        public string MonthName
        {
            get => _monthName;
            set => SetProperty(ref _monthName, value);
        }

        #endregion


        #region AddAppointmentCommand

        public DelegateCommand<Day> AddAppointmentCommand { get; }

        private void AddAppointment(Day day)
        {
            var parameters = new DialogParameters
            {
                { "title", $"{day.Date:dd.MM.yyyy}, {TranslateWeekday(day.Date.DayOfWeek.ToString())}" },
                { "day", day }
            };
            _dialogService.ShowDialog("TimetableDialog", parameters);
        }

        #endregion

        #region ViewAppointmentCommand

        public DelegateCommand<Appointment> ViewAppointmentCommand { get; }

        private void ViewAppointment(Appointment appointment)
        {
            if (appointment.LinkOrText == null && !appointment.IsFreeDay)
            {
                EditAppointment(appointment);
            }

            else
            {
                ViewClientInfo(appointment);
            }
        }

        private void EditAppointment(Appointment appointment)
        {
            var date = Days.Where(day => day.Id == appointment.DayId).FirstOrDefault().Date;
            var parameters = new DialogParameters
            {
                { "title", $"{date.Date:dd.MM.yyyy}, {TranslateWeekday(date.Date.DayOfWeek.ToString())}" },
                { "appointment", appointment }
            };
            _dialogService.ShowDialog("AppointmentDialog", parameters);
        }

        private void ViewClientInfo(Appointment appointment)
        {
            if (appointment.LinkOrText != null && UriHelper.IsLink(appointment.LinkOrText))
            {
                Launcher.OpenAsync(new Uri(appointment.LinkOrText));
            }
            else
            {
                var date = Days.Where(day => day.Id == appointment.DayId).FirstOrDefault().Date;
                var message = appointment.LinkOrText;
                if (appointment.IsFreeDay && appointment.LinkOrText == null)
                {
                    message = "Отдыхайте ٩(◕‿◕)۶";
                }
                var parameters = new DialogParameters
                {
                    { "title", $"{date.Date:dd.MM.yyyy}, {TranslateWeekday(date.Date.DayOfWeek.ToString())}" },
                    { "message", message }
                };
                _dialogService.ShowDialog("AlertDialog", parameters);
            }
        }

        #endregion

        #region LoadNextMonthCommand
        public DelegateCommand LoadNextMonthCommand { get; }

        private async void LoadNextMonth()
        {
            _currrentDateTime = _currrentDateTime.AddMonths(1);

            await LoadMonthAsync(_currrentDateTime);
        }

        #endregion

        #region LoadPreviousMonthCommand

        public DelegateCommand LoadPreviousMonthCommand { get; }

        private async void LoadPreviousMonth()
        {
            _currrentDateTime = _currrentDateTime.AddMonths(-1);

            await LoadMonthAsync(_currrentDateTime);
        }

        #endregion

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

        private string TranslateWeekday(string day)
        {
            return Constants.DaysDictionary[day];
        }
    }
}

