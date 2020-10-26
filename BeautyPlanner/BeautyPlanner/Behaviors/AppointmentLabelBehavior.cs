using System.ComponentModel;
using BeautyPlanner.Models;
using Xamarin.Forms;

namespace BeautyPlanner.Behaviors
{
    public class AppointmentLabelBehavior : Behavior<Label>
    {
        public string Format
        {
            get => (string)GetValue(FormatProperty);
            set => SetValue(FormatProperty, value);
        }

        public static readonly BindableProperty FormatProperty =
            BindableProperty.Create(nameof(Format), typeof(string), 
                typeof(AppointmentLabelBehavior), default(string), BindingMode.TwoWay);


        protected override void OnAttachedTo(Label bindable)
        {
            bindable.PropertyChanged += OnTextPropertyChanged;
            base.OnAttachedTo(bindable);
        }

        private void OnTextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var label = (Label) sender;
            var appointment = (Appointment) label.BindingContext;
            var text = appointment.Time.ToString("hh\\:mm");
            var freeDay = appointment.AppointmentType;

            label.PropertyChanged -= OnTextPropertyChanged;

            switch (Format)
            {
                case Constants.FullPedicure: label.Text = $"{text} (пп)";
                    SetStrikethrough(label, appointment);
                    break;
                case Constants.ManicurePedicure: label.Text = $"{text} (м+п)";
                    SetStrikethrough(label, appointment);
                    break;
                case Constants.Pedicure: label.Text = $"{text} (п)";
                    SetStrikethrough(label, appointment);
                    break;
                case Constants.FreeDay: label.Text = freeDay;
                    break;
                case Constants.ImportantFreeDay: label.Text = freeDay;
                    label.TextColor = Color.DarkRed;
                    break;
                default: label.Text = text;
                    SetStrikethrough(label, appointment);
                    break;
            }

            label.PropertyChanged += OnTextPropertyChanged;
        }

        protected override void OnDetachingFrom(Label bindable)
        {
            bindable.PropertyChanged -= OnTextPropertyChanged;
            base.OnDetachingFrom(bindable);
        }

        private void SetStrikethrough(Label label, Appointment appointment)
        {
            if (appointment.LinkOrText != null)
            {
                label.TextDecorations = TextDecorations.Strikethrough;
            }
        }
    }
}
