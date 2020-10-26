using System;
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
    class AppointmentDialogViewModel : BindableBase, IDialogAware, IAutoInitialize
    {
        private IDatabaseService _dbService;
        private Appointment _tempAppointment;

        public AppointmentDialogViewModel(IDatabaseService dbService)
        {
            _dbService = dbService;

            CloseCommand = new DelegateCommand(() => RequestClose(null));

            SaveChangesCommand = new DelegateCommand(SaveChanges);
            CancelCommand = new DelegateCommand(Cancel);
        }

        public ObservableCollection<string> ProcedureTypes { get; set; } = new ObservableCollection<string>
        {
            Constants.Manicure,
            Constants.FullPedicure,
            Constants.ManicurePedicure,
            Constants.Pedicure
        };

        #region Title property

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        #endregion

        #region Appointment property

        private Appointment _appointment;
        [AutoInitialize(true)]
        public Appointment Appointment
        {
            get => _appointment;
            set => SetProperty(ref _appointment, value);
        }

        #endregion

        public DelegateCommand CloseCommand { get; }
        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            //
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _tempAppointment = parameters.GetValues<Appointment>("appointment").FirstOrDefault();
        }

        public event Action<IDialogParameters> RequestClose;

        #region SaveChangesCommand

        public DelegateCommand SaveChangesCommand { get; }

        private async void SaveChanges()
        {
            await _dbService.SaveAppointmentAsync(Appointment);
            CloseCommand.Execute();
        }

        #endregion

        #region CancelCommand

        public DelegateCommand CancelCommand { get; }

        private void Cancel()
        {
            //do logic
            CloseCommand.Execute();
        }

        #endregion

    }
}
