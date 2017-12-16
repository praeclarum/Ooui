using System;
using System.ComponentModel;
using Xamarin.Forms;
using Ooui.Forms.Extensions;

namespace Ooui.Forms.Renderers
{
    public class TimePickerRenderer : ViewRenderer<TimePicker, Input>
    {
        bool _disposed;

        IElementController ElementController => Element as IElementController;

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            var size = "00:00:00".MeasureSize(string.Empty, 16.0, FontAttributes.None);
            size = new Size(size.Width, size.Height * 1.428 + 14);
            return new SizeRequest(size, size);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (Control == null)
            {
                var entry = new Input
                {
                    ClassName = "form-control",
                    Type = InputType.Time
                };

                entry.Change += OnEnded;

                SetNativeControl(entry);
            }

            UpdateTime(false);
            UpdateTextColor();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == TimePicker.TimeProperty.PropertyName || e.PropertyName == DatePicker.FormatProperty.PropertyName)
                UpdateTime(true);
            else if (e.PropertyName == TimePicker.TextColorProperty.PropertyName || e.PropertyName == DatePicker.FormatProperty.PropertyName)
                UpdateTextColor();
        }

        void OnEnded(object sender, EventArgs eventArgs)
        {
            DateTime.TryParseExact(Control.Value, @"hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out var date);
            ElementController?.SetValueFromRenderer(TimePicker.TimeProperty, date);
            ElementController?.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
        }

        void UpdateTime(bool animate)
        {
            Control.Value = Element.Time.ToString(@"hh\:mm\:ss");
        }

        void UpdateTextColor()
        {
            var textColor = (Xamarin.Forms.Color)Element.GetValue(TimePicker.TextColorProperty);

            Control.Style.Color = textColor.ToOouiColor(Xamarin.Forms.Color.Black);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing)
            {
                if (Control != null)
                {
                    Control.Change -= OnEnded;
                }
            }

            base.Dispose(disposing);
        }
    }
}
