using System;
using System.ComponentModel;
using Xamarin.Forms;
using Ooui.Forms.Extensions;

namespace Ooui.Forms.Renderers
{
    public class DatePickerRenderer : ViewRenderer<DatePicker, Input>
    {
        bool _disposed;

        IElementController ElementController => Element as IElementController;

        public override SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
        {
            var size = "00/00/0000".MeasureSize ("", 16.0, FontAttributes.None);
            size = new Size (size.Width, size.Height * 1.428 + 14);
            return new SizeRequest (size, size);
        }

        protected override void OnElementChanged (ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged (e);

            if (e.NewElement == null)
                return;

            if (Control == null) {
                var entry = new Input {
                    ClassName = "form-control",
                    Type = InputType.Date,
                };

                //entry.Input += OnStarted;
                entry.Change += OnEnded;

                SetNativeControl (entry);
            }

            UpdateDateFromModel (false);
            UpdateMaximumDate ();
            UpdateMinimumDate ();
            UpdateTextColor ();
            UpdateFlowDirection ();
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);

            if (e.PropertyName == DatePicker.DateProperty.PropertyName || e.PropertyName == DatePicker.FormatProperty.PropertyName)
                UpdateDateFromModel (true);
            else if (e.PropertyName == DatePicker.MinimumDateProperty.PropertyName)
                UpdateMinimumDate ();
            else if (e.PropertyName == DatePicker.MaximumDateProperty.PropertyName)
                UpdateMaximumDate ();
            else if (e.PropertyName == DatePicker.TextColorProperty.PropertyName || e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                UpdateTextColor ();
        }

        void OnEnded (object sender, EventArgs eventArgs)
        {
            DateTime.TryParseExact (Control.Value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out var date);
            ElementController?.SetValueFromRenderer (DatePicker.DateProperty, date);
            ElementController?.SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, false);
        }

        void OnStarted (object sender, EventArgs eventArgs)
        {
            ElementController?.SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, true);
        }

        void UpdateDateFromModel (bool animate)
        {
            Control.Value = Element.Date.ToString ("yyyy-MM-dd");
        }

        void UpdateFlowDirection ()
        {
        }

        void UpdateMaximumDate ()
        {
        }

        void UpdateMinimumDate ()
        {
        }

        void UpdateTextColor ()
        {
        }

        protected override void Dispose (bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing) {
                if (Control != null) {
                    //Control.Input -= OnStarted;
                    Control.Change -= OnEnded;
                }
            }

            base.Dispose (disposing);
        }
    }
}
