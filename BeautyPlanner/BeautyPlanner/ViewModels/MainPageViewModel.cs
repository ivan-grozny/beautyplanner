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
            ShowTimetableCommand = new DelegateCommand(ShowTimetable);

            _databaseService = databaseService;
            _dialogService = dialogService;

            LoadMonthAsync(DateTime.Now).SafeFireAndForget(true, e => LoadMonthAsync(DateTime.Now));
            _currrentDateTime = DateTime.Now;

            //AddAppointmentCommand = new DelegateCommand<Day>(AddAppointment);
            //ViewAppointmentCommand = new DelegateCommand<Appointment>(ViewAppointment);

            SwitchToAppointmentsCommand = new DelegateCommand(SwitchToAppointments);
            SwitchToNewAppointmentCommand = new DelegateCommand(SwitchToNewAppointment);

            LoadNextMonthCommand = new DelegateCommand(LoadNextMonth);
            LoadPreviousMonthCommand = new DelegateCommand(LoadPreviousMonth);

            SelectDayCommand = new DelegateCommand<Day>(SelectDay);
        }

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

        #region IsAppointmentsTabVisible property

        private bool _isAppointmentsTabVisible = true;
        public bool IsAppointmentsTabVisible
        {
            get => _isAppointmentsTabVisible;
            set => SetProperty(ref _isAppointmentsTabVisible, value);
        }

        #endregion

        #region FrameHeight property

        private int _frameHeight;
        public int FrameHeight
        {
            get => _frameHeight;
            set => SetProperty(ref _frameHeight, value);
        }

        private void SetFrameHeight()
        {
            var date = new DateTime(_currrentDateTime.Year, _currrentDateTime.Month, 1); 
            var weekday = date.DayOfWeek;
            var firstWeekDays = 0;
            switch (weekday)
            {
                case DayOfWeek.Tuesday:
                    firstWeekDays = 6;
                    break;
                case DayOfWeek.Wednesday:
                    firstWeekDays = 5;
                    break;
                case DayOfWeek.Thursday:
                    firstWeekDays = 4;
                    break;
                case DayOfWeek.Friday:
                    firstWeekDays = 3;
                    break;
                case DayOfWeek.Saturday:
                    firstWeekDays = 2;
                    break;
                case DayOfWeek.Sunday:
                    firstWeekDays = 1;
                    break;
            }

            var weekdays = Days.Count - firstWeekDays;
            var weeks = weekdays / 7.0;
            if (!IsInteger(weeks))
            {
                weeks += 1;
            }

            if (firstWeekDays != 0)
            {
                weeks += 1;
            }

            switch ((int)weeks)
            {
                case 6: 
                    FrameHeight = 336;
                    break;
                case 5:
                    FrameHeight = 280;
                    break;
                case 4:
                    FrameHeight = 224;
                    break;
            }
        }

        private bool IsInteger(double number)
        {
            return (number % 1 == 0);
        }

        #endregion

        #region SwitchToAppointmentsCommand
        public DelegateCommand SwitchToAppointmentsCommand { get; }
        private void SwitchToAppointments()
        {
            IsAppointmentsTabVisible = true;
        }

        #endregion

        #region SwitchToNewAppointmentCommand
        public DelegateCommand SwitchToNewAppointmentCommand { get; }

        private void SwitchToNewAppointment()
        {
            IsAppointmentsTabVisible = false;
        }

        #endregion

        //#region AddAppointmentCommand

        //public DelegateCommand<Day> AddAppointmentCommand { get; }

        //private void AddAppointment(Day day)
        //{
        //    var parameters = new DialogParameters
        //    {
        //        { "title", $"{day.Date:dd.MM.yyyy}, {TranslateWeekday(day.Date.DayOfWeek.ToString())}" },
        //        { "day", day }
        //    };
        //    _dialogService.ShowDialog("TimetableDialog", parameters);
        //}

        //#endregion

        //#region ViewAppointmentCommand

        //public DelegateCommand<Appointment> ViewAppointmentCommand { get; }

        //private void ViewAppointment(Appointment appointment)
        //{
        //    if (appointment.LinkOrText == null && !appointment.IsFreeDay)
        //    {
        //        EditAppointment(appointment);
        //    }

        //    else
        //    {
        //        ViewClientInfo(appointment);
        //    }
        //}

        //private void EditAppointment(Appointment appointment)
        //{
        //    var date = Days.Where(day => day.Id == appointment.DayId).FirstOrDefault().Date;
        //    var parameters = new DialogParameters
        //    {
        //        { "title", $"{date.Date:dd.MM.yyyy}, {TranslateWeekday(date.Date.DayOfWeek.ToString())}" },
        //        { "appointment", appointment }
        //    };
        //    _dialogService.ShowDialog("AppointmentDialog", parameters);
        //}

        //private void ViewClientInfo(Appointment appointment)
        //{
        //    if (appointment.LinkOrText != null && UriHelper.IsLink(appointment.LinkOrText))
        //    {
        //        Launcher.OpenAsync(new Uri(appointment.LinkOrText));
        //    }
        //    else
        //    {
        //        var date = Days.Where(day => day.Id == appointment.DayId).FirstOrDefault().Date;
        //        var message = appointment.LinkOrText;
        //        if (appointment.IsFreeDay && appointment.LinkOrText == null)
        //        {
        //            message = "Отдыхайте ٩(◕‿◕)۶";
        //        }
        //        var parameters = new DialogParameters
        //        {
        //            { "title", $"{date.Date:dd.MM.yyyy}, {TranslateWeekday(date.Date.DayOfWeek.ToString())}" },
        //            { "message", message }
        //        };
        //        _dialogService.ShowDialog("AlertDialog", parameters);
        //    }
        //}

        //#endregion

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

        #region SelectDayCommand

        public DelegateCommand<Day> SelectDayCommand { get; }

        private void SelectDay(Day day)
        {
            MarkDaySelected(day);
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

            SetFrameHeight();
            SetDefaultSelectedDay();
        }

        private void SetDefaultSelectedDay()
        {
            if (_currrentDateTime.Month == DateTime.Now.Month && _currrentDateTime.Year == DateTime.Now.Year)
            {
                SelectedDay = Days.FirstOrDefault(d => d.Date.Day == DateTime.Now.Day);
                MarkDaySelected(SelectedDay);
            }
            else
            {
                SelectedDay = Days.FirstOrDefault();
                MarkDaySelected(SelectedDay);
            }
        }

        private void MarkDaySelected(Day selectedDay)
        {
            var currentSelectedDay = Days.FirstOrDefault(d => d.IsSelected);
            if (currentSelectedDay != null)
            {
                currentSelectedDay.IsSelected = false;
            }

            selectedDay.IsSelected = true;
        }

        private string TranslateWeekday(string day)
        {
            return Constants.DaysDictionary[day];
        }

        #region SelectedDay property

        private Day _selectedDay;
       
        public Day SelectedDay
        {
            get => _selectedDay;
            set => SetProperty(ref _selectedDay, value);
        }

        #endregion
    }
}

