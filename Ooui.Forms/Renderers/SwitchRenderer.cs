using System;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class SwitchRenderer : ViewRenderer<Switch, Input>
    {
        public override SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
        {
            var size = new Size (54, 38);
            return new SizeRequest (size, size);
        }

        protected override void Dispose (bool disposing)
        {
            if (disposing)
                Control.Changed -= OnControlValueChanged;

            base.Dispose (disposing);
        }

        protected override void OnElementChanged (ElementChangedEventArgs<Switch> e)
        {
            if (e.OldElement != null)
                e.OldElement.Toggled -= OnElementToggled;

            if (e.NewElement != null) {
                if (Control == null) {
                    var input = new Input (InputType.Checkbox);
                    input.SetAttribute ("data-toggle", "toggle");
                    SetNativeControl (input);
                    input.Call ("$.bootstrapToggle");
                    Control.Changed += OnControlValueChanged;
                }

                Control.IsChecked = Element.IsToggled;
                e.NewElement.Toggled += OnElementToggled;
            }

            base.OnElementChanged (e);
        }

        void OnControlValueChanged (object sender, EventArgs e)
        {
            ((IElementController)Element).SetValueFromRenderer (Switch.IsToggledProperty, Control.IsChecked);
        }

        void OnElementToggled (object sender, EventArgs e)
        {
            Control.IsChecked = Element.IsToggled;
        }
    }
}
