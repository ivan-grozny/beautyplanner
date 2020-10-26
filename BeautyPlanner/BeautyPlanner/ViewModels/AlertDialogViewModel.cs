using System;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace BeautyPlanner.ViewModels
{
    public class AlertDialogViewModel : BindableBase, IDialogAware, IAutoInitialize
    {
        public AlertDialogViewModel()
        {
            CloseCommand = new DelegateCommand(() => RequestClose(null));
        }
        public DelegateCommand CloseCommand { get; }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            //
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            //
        }

        public event Action<IDialogParameters> RequestClose;

        #region Title property

        private string _title;
        [AutoInitialize(true)]
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        #endregion

        #region Message property

        private string _message;
        [AutoInitialize(true)]
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        #endregion
    }
}
