using Ooui.Html;
using System.ComponentModel;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class SliderRenderer : ViewRenderer<Slider, Input>
    {
        private bool _disposed;

        IElementController ElementController => Element as IElementController;

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            var size = new Size(100, 38);
            return new SizeRequest(size, size);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (Control == null)
            {
                var range = new Input
                {
                    ClassName = "form-control",
                    Type = InputType.Range
                };

                range.Change += OnValueChange;

                SetNativeControl(range);
            }

            UpdateMaximum();
            UpdateMinimum();
            UpdateValue();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Slider.MaximumProperty.PropertyName)
                UpdateMaximum();
            else if (e.PropertyName == Slider.MinimumProperty.PropertyName)
                UpdateMinimum();
            else if (e.PropertyName == Slider.ValueProperty.PropertyName)
                UpdateValue();
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
                    Control.Change -= OnValueChange;
                }
            }

            base.Dispose(disposing);
        }

        private void UpdateMaximum()
        {
            Control.Maximum = Element.Maximum;
        }

        private void UpdateMinimum()
        {
            Control.Minimum = Element.Minimum;
        }

        private void UpdateValue()
        {
            Control.NumberValue = Element.Value;
        }

        private void OnValueChange(object sender, TargetEventArgs e)
        {
            ElementController.SetValueFromRenderer(Slider.ValueProperty, Control.NumberValue);
        }
    }
}
