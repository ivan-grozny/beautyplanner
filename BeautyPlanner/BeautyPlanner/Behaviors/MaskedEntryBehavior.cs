﻿using System.Collections.Generic;
using Xamarin.Forms;

namespace BeautyPlanner.Behaviors
{
    public class MaskedEntryBehavior : Behavior<Entry>
    {
        private IDictionary<int, char> _positions;

        public string Mask
        {
            get => (string)GetValue(MaskProperty);
            set => SetValue(MaskProperty, value);
        }

        public static readonly BindableProperty MaskProperty =
            BindableProperty.Create(nameof(Mask), typeof(string), typeof(MaskedEntryBehavior), default(string), BindingMode.Default, null, OnMaskPropertyChanged);

        private static void OnMaskPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is MaskedEntryBehavior behavior)
            {
                behavior.SetPositions();
            }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(bindable);
        }

        protected void SetPositions()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                _positions = null;
                return;
            }

            var list = new Dictionary<int, char>();
            for (var i = 0; i < Mask.Length; i++)
            {
                if (Mask[i] != 'X')
                {
                    list.Add(i, Mask[i]);
                }
            }
            _positions = list;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (!(sender is Entry entry))
            {
                return;
            }

            var text = entry.Text;

            if (string.IsNullOrWhiteSpace(text) || _positions == null)
            {
                return;
            }

            if (text.Length > Mask.Length)
            {
                entry.Text = text.Remove(text.Length - 1);
                return;
            }

            foreach (var position in _positions)
            {
                if (text.Length >= position.Key + 1)
                {
                    var value = position.Value.ToString();
                    if (text.Substring(position.Key, 1) != value)
                    {
                        text = text.Insert(position.Key, value);
                    }
                }
            }

            if (entry.Text != text)
            {
                entry.Text = text;
            }
        }
    }
}

