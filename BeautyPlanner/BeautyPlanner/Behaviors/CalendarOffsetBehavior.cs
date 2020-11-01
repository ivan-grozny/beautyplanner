using System;
using BeautyPlanner.Models;
using Xamarin.Forms;

namespace BeautyPlanner.Behaviors
{
    public class CalendarOffsetBehavior: Behavior<Label>
    {
        protected override void OnAttachedTo(Label bindable)
        {
            bindable.BindingContextChanged += OnLabelTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Label bindable)
        {
            bindable.BindingContextChanged -= OnLabelTextChanged;
            base.OnDetachingFrom(bindable);
        }

        private void OnLabelTextChanged(object sender, EventArgs e)
        {
            var label = (Label) sender;
            var day = (Day)label.BindingContext;
            var date = day.Date;
            if (date.Day == 1)
            {
                var weekday = date.DayOfWeek;
                switch (weekday)
                {
                    case DayOfWeek.Tuesday: 
                        label.Margin = new Thickness(59, 3, 3, 3);
                        break;
                    case DayOfWeek.Wednesday:
                        label.Margin = new Thickness(115, 3, 3, 3);
                        break;
                    case DayOfWeek.Thursday:
                        label.Margin = new Thickness(171, 3, 3, 3);
                        break;
                    case DayOfWeek.Friday:
                        label.Margin = new Thickness(227, 3, 3, 3);
                        break;
                    case DayOfWeek.Saturday:
                        label.Margin = new Thickness(283, 3, 3, 3);
                        break;
                    case DayOfWeek.Sunday:
                        label.Margin = new Thickness(339, 3, 3, 3);
                        break;
                }
            }
        }
    }
}
