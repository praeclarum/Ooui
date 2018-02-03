using System;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class SwitchRenderer : ViewRenderer<Switch, SwitchRenderer.SwitchElement>
    {
        public override SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
        {
            var size = new Size (54, 38);
            return new SizeRequest (size, size);
        }

        protected override void Dispose (bool disposing)
        {
            if (disposing)
                Control.Change -= OnControlValueChanged;

            base.Dispose (disposing);
        }

        protected override void OnElementChanged (ElementChangedEventArgs<Switch> e)
        {
            if (e.OldElement != null)
                e.OldElement.Toggled -= OnElementToggled;

            if (e.NewElement != null) {
                if (Control == null) {
                    var input = new SwitchElement ();
                    SetNativeControl (input);
                    Control.Change += OnControlValueChanged;
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

        public class SwitchElement : Div
        {
            public event EventHandler Change;
            bool isChecked = false;
            readonly Div knob = new Div ();
            public bool IsChecked {
                get => isChecked;
                set {
                    isChecked = value;
                    UpdateUI ();
                }
            }
            public SwitchElement ()
            {
                AppendChild (knob);
                knob.Style.Position = "absolute";
                knob.Style.BorderRadius = "10px";
                knob.Style.Cursor = "pointer";
                knob.Style.Top = "2px";
                knob.Style.Width = "18px";
                knob.Style.Height = "34px";

                Style.BorderRadius = "10px";
                Style.Cursor = "pointer";
                Style.BorderStyle = "solid";
                Style.BorderWidth = "2px";
                Click += (s, e) => {
                    IsChecked = !IsChecked;
                    Change?.Invoke (this, EventArgs.Empty);
                };
                UpdateUI ();
            }

            void UpdateUI ()
            {
                Style.BackgroundColor = isChecked ? "#337ab7" : "#888";
                Style.BorderColor = Style.BackgroundColor;
                knob.Style.BackgroundColor = isChecked ? "#FFF" : "#EEE";
                if (isChecked) {
                    knob.Style.Left = "34px";
                }
                else {
                    knob.Style.Left = "2px";
                }
            }
        }
    }
}
