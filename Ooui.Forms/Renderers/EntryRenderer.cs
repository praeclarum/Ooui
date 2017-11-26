using System;
using System.ComponentModel;
using System.Diagnostics;
using Ooui.Forms.Extensions;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class EntryRenderer : ViewRenderer<Entry, Ooui.Input>
    {
        Ooui.Color _defaultTextColor;
        bool _disposed;

        static Size initialSize = Size.Zero;

        IElementController ElementController => Element as IElementController;

        public override SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
        {
            var size = Element.Text.MeasureSize (Element.FontFamily, Element.FontSize, Element.FontAttributes);
            size = new Size (size.Width, size.Height * 1.428 + 14);
            return new SizeRequest (size, size);
        }

        protected override void Dispose (bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing) {
                if (Control != null) {
                    //Control.Inputted -= OnEditingBegan;
                    Control.Inputted -= OnEditingChanged;
                    Control.Changed -= OnEditingEnded;
                }
            }

            base.Dispose (disposing);
        }

        protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged (e);

            if (e.NewElement == null)
                return;

            if (Control == null) {
                var textField = new Ooui.Input (InputType.Text);
                SetNativeControl (textField);

                Debug.Assert (Control != null, "Control != null");

                textField.ClassName = "form-control";

                _defaultTextColor = Colors.Black;

                textField.Inputted += OnEditingChanged;

                //textField.EditingDidBegin += OnEditingBegan;
                textField.Changed += OnEditingEnded;
            }

            UpdatePlaceholder ();
            UpdatePassword ();
            UpdateText ();
            UpdateColor ();
            UpdateFont ();
            UpdateKeyboard ();
            UpdateAlignment ();
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
            if (Control.Text != Element.Text) {
                ElementController.SetValueFromRenderer (Entry.TextProperty, Control.Text);
            }

            ElementController.SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, false);
        }

        void UpdateAlignment ()
        {
            Control.Style.TextAlign = Element.HorizontalTextAlignment.ToOouiTextAlign ();
        }

        void UpdateColor ()
        {
            var textColor = Element.TextColor;

            if (textColor.IsDefault || !Element.IsEnabled)
                Control.Style.Color = _defaultTextColor;
            else
                Control.Style.Color = textColor.ToOouiColor ();
        }

        void UpdateFont ()
        {
            if (initialSize == Size.Zero) {
                var testString = "Tj";
                initialSize = testString.MeasureSize (Control.Style);
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
