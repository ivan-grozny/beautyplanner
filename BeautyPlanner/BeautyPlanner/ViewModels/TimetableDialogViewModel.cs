using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BeautyPlanner.Interfaces;
using BeautyPlanner.Models;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace BeautyPlanner.ViewModels
{
    public class TimetableDialogViewModel : BindableBase, IDialogAware, IAutoInitialize
    {
        private ObservableCollection<Appointment> _tempAppointments;
        private List<Appointment> _deletedAppointments = new List<Appointment>();
        private readonly IDatabaseService _dbService;
        private readonly IDialogService _dialogService;

        public TimetableDialogViewModel(IDatabaseService dbService, IDialogService dialogService)
        {
            _dbService = dbService;
            _dialogService = dialogService;

            CloseCommand = new DelegateCommand(() => RequestClose(null));

            SwitchToFreeDayCommand = new DelegateCommand(SwitchToFreeDay);
            SwitchToAppointmentsCommand = new DelegateCommand(SwitchToAppointments);

            AddAppointmentCommand = new DelegateCommand(AddAppointment);
            CancelCommand = new DelegateCommand(Cancel);
            SaveAppointmentsCommand = new DelegateCommand(SaveAppointments);
            DeleteAppointmentCommand = new DelegateCommand<Appointment>(DeleteAppointment);
        }

        #region TempFreeDay property
        private Appointment _tempFreeDay;
        public Appointment TempFreeDay
        {
            get => _tempFreeDay;
            set => SetProperty(ref _tempFreeDay, value);
        }
        #endregion

        public ObservableCollection<string> FreeDayTypes { get; set; } = new ObservableCollection<string>
        {
            Constants.FreeDay,
            Constants.ImportantFreeDay
        };

        #region DeleteAppointmentCommand

        public DelegateCommand<Appointment> DeleteAppointmentCommand { get; }

        private void DeleteAppointment(Appointment appointment)
        {
            Day.Appointments.Remove(appointment);
            _deletedAppointments.Add(appointment);
        }

        #endregion

        #region SwitchToFreeDayCommand
        public DelegateCommand SwitchToFreeDayCommand { get; }

        private void SwitchToFreeDay()
        {
            IsAppointmentsTabVisible = false;
            TempFreeDay = new Appointment{AppointmentType = Constants.FreeDay};
        }

        #endregion

        #region SwitchToAppointmentsCommand
        public DelegateCommand SwitchToAppointmentsCommand { get; }
        private void SwitchToAppointments()
        {
            IsAppointmentsTabVisible = true;
            TempFreeDay = null;
        }

        #endregion

        #region AddAppointmentCommand
        public DelegateCommand AddAppointmentCommand { get; }
        private void AddAppointment()
        {
            var appointment = new Appointment();
            Day.Appointments.Add(appointment);
        }

        #endregion

        #region CancelCommand
        public DelegateCommand CancelCommand { get; }

        private void Cancel()
        {
            _deletedAppointments.Clear();

            Day.Appointments.Clear();
            foreach (var ta in _tempAppointments)
            {
                Day.Appointments.Add(ta);
            }
            CloseCommand.Execute();
        }

        #endregion

        #region SaveAppointmentsCommand

        public DelegateCommand SaveAppointmentsCommand { get; }

        private async void SaveAppointments()
        {
            if (TempFreeDay == null)
            {
                await _dbService.SaveAppointmentsAsync(Day.Appointments, Day.Id, Day.MonthId);
            }
            else if (TempFreeDay != null && CheckAppointmentsExist())
            {
                //var parameters = new DialogParameters
                //{
                //    { "title", "Ошибка!" },
                //    { "message", "Вы не можете сделать день выходным пока не удалите все назначенные процедуры" }
                //};
                //_dialogService.ShowDialog("AlertDialog", parameters);
            }
            else
            {
                TempFreeDay.DayId = Day.Id;
                TempFreeDay.MonthId = Day.MonthId;
                TempFreeDay.IsFreeDay = true;

                _deletedAppointments.AddRange(Day.Appointments);
                Day.Appointments.Clear();
                Day.Appointments.Add(TempFreeDay);

                await _dbService.SaveAppointmentAsync(TempFreeDay);
            }

            foreach (var deletedAppointment in _deletedAppointments)
            {
                await _dbService.DeleteAppointmentAsync(deletedAppointment);
            }
            _deletedAppointments.Clear();
            CloseCommand.Execute();
        }

        #endregion

        #region Day property

        private Day _day;
        [AutoInitialize(true)]
        public Day Day
        {
            get => _day;
            set => SetProperty(ref _day, value);
        }

        #endregion

        #region Title property

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
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
        
        public DelegateCommand CloseCommand { get; }

        public event Action<IDialogParameters> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            //
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Day = parameters.GetValue<Day>("day");
            _tempAppointments = new ObservableCollection<Appointment>(Day.Appointments);
            if (!Day.Appointments.Any())
            {
                Day.Appointments.Add(new Appointment { Time = new TimeSpan(18, 0, 0) });
            }
        }

        private bool CheckAppointmentsExist()
        {
            foreach (var a in Day.Appointments)
            {
                if (a.LinkOrText != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
