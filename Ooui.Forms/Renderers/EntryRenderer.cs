using System;
using System.ComponentModel;
using System.Diagnostics;
using Ooui.Forms.Extensions;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class EntryRenderer : ViewRenderer<Entry, Ooui.TextInput>
    {
        bool _disposed;

        static Size initialSize = Size.Zero;

        IElementController ElementController => Element as IElementController;

        public override SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
        {
            var text = Element.Text;
            if (text == null || text.Length == 0) {
                text = Element.Placeholder;
            }
            if (text == null || text.Length == 0) {
                text = " ";
            }
            var size = text.MeasureSize (Element.FontFamily, Element.FontSize, Element.FontAttributes, widthConstraint, heightConstraint);
            var vpadding = 16;
            var hpadding = 32;
            size = new Size (size.Width + hpadding, size.Height + vpadding);
            return new SizeRequest (size, size);
        }

        protected override void Dispose (bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing) {
                if (Element != null) {
                    Element.FocusChangeRequested -= Element_FocusChangeRequested;
                }
                if (Control != null) {
                    Control.Input -= OnEditingChanged;
                    Control.Change -= OnEditingEnded;
                }
            }

            base.Dispose (disposing);
        }

        protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged (e);

            if (e.OldElement != null) {
                e.OldElement.FocusChangeRequested -= Element_FocusChangeRequested;
            }

            if (e.NewElement == null)
                return;

            e.NewElement.FocusChangeRequested += Element_FocusChangeRequested;

            if (Control == null) {
                var textField = new Ooui.TextInput ();
                SetNativeControl (textField);

                Debug.Assert (Control != null, "Control != null");

                textField.ClassName = "form-control";

                textField.Input += OnEditingChanged;
                textField.Change += OnEditingEnded;
            }

            UpdatePlaceholder ();
            UpdatePassword ();
            UpdateText ();
            UpdateColor ();
            UpdateFont ();
            UpdateKeyboard ();
            UpdateAlignment ();
        }

        void Element_FocusChangeRequested (object sender, VisualElement.FocusRequestArgs e)
        {
            if (e.Focus && Control != null) {
                Control?.Focus ();
                e.Result = true;
            }
        }


        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Entry.PlaceholderProperty.PropertyName || e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
                UpdatePlaceholder ();
            else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
                UpdatePassword ();
            else if (e.PropertyName == Entry.TextProperty.PropertyName)
                UpdateText ();
            else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
                UpdateColor ();
            else if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
                UpdateKeyboard ();
            else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
                UpdateAlignment ();
            else if (e.PropertyName == Entry.FontAttributesProperty.PropertyName)
                UpdateFont ();
            else if (e.PropertyName == Entry.FontFamilyProperty.PropertyName)
                UpdateFont ();
            else if (e.PropertyName == Entry.FontSizeProperty.PropertyName)
                UpdateFont ();
            else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName) {
                UpdateColor ();
                UpdatePlaceholder ();
            }

            base.OnElementPropertyChanged (sender, e);
        }

        void OnEditingBegan (object sender, EventArgs e)
        {
            ElementController.SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, true);
        }

        void OnEditingChanged (object sender, EventArgs eventArgs)
        {
            ElementController.SetValueFromRenderer (Entry.TextProperty, Control.Value);
        }

        void OnEditingEnded (object sender, EventArgs e)
        {
            // Typing aid changes don't always raise EditingChanged event
            if (Control.Value != Element.Text) {
                ElementController.SetValueFromRenderer (Entry.TextProperty, Control.Value);
            }

            ElementController.SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, false);
        }

        void UpdateAlignment ()
        {
            Control.Style.TextAlign = Element.HorizontalTextAlignment.ToOouiTextAlign ();
        }

        void UpdateColor ()
        {
            Control.Style.Color = Element.TextColor.ToOouiColor (OouiTheme.TextColor);
        }

        void UpdateFont ()
        {
            if (initialSize == Size.Zero) {
                var testString = "Tj";
                initialSize = testString.MeasureSize (Control.Style, double.PositiveInfinity, double.PositiveInfinity);
            }

            Element.SetStyleFont (Element.FontFamily, Element.FontSize, Element.FontAttributes, Control.Style);
        }

        void UpdateKeyboard ()
        {
        }

        void UpdatePassword ()
        {
            Control.Type = Element.IsPassword ? InputType.Password : InputType.Text;
        }

        void UpdatePlaceholder ()
        {
            Control.Placeholder = Element.Placeholder ?? "";
        }

        void UpdateText ()
        {
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (Control.Value != Element.Text)
                Control.Value = Element.Text;
        }
    }
}
