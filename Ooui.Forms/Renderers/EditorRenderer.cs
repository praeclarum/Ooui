using System;
using System.ComponentModel;
using Xamarin.Forms;
using Ooui.Forms.Extensions;

namespace Ooui.Forms.Renderers
{
    public class EditorRenderer : ViewRenderer<Editor, TextArea>
    {
        bool _disposed;
        IEditorController ElementController => Element;

        public override SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
        {
            var size = new Size (160, 100);
            return new SizeRequest (size, size);
        }

        protected override void Dispose (bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing) {
                if (Control != null) {
                    Control.Input -= HandleChanged;
                    //Control.Started -= OnStarted;
                    Control.Change -= OnEnded;
                }
            }

            base.Dispose (disposing);
        }

        protected override void OnElementChanged (ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged (e);

            if (e.NewElement == null)
                return;

            if (Control == null) {
                var textArea = new TextArea {
                    ClassName = "form-control"
                };
                textArea.Style["resize"] = "none";
                SetNativeControl (textArea);

                Control.Input += HandleChanged;
                //Control.Started += OnStarted;
                Control.Change += OnEnded;
            }

            UpdateText ();
            UpdateFont ();
            UpdateTextColor ();
            UpdateKeyboard ();
            UpdateEditable ();
            UpdateTextAlignment ();
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);

            if (e.PropertyName == Editor.TextProperty.PropertyName)
                UpdateText ();
            else if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
                UpdateKeyboard ();
            else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                UpdateEditable ();
            else if (e.PropertyName == Editor.TextColorProperty.PropertyName)
                UpdateTextColor ();
            else if (e.PropertyName == Editor.FontAttributesProperty.PropertyName)
                UpdateFont ();
            else if (e.PropertyName == Editor.FontFamilyProperty.PropertyName)
                UpdateFont ();
            else if (e.PropertyName == Editor.FontSizeProperty.PropertyName)
                UpdateFont ();
        }

        void HandleChanged (object sender, EventArgs e)
        {
            ElementController.SetValueFromRenderer (Editor.TextProperty, Control.Value);
        }

        void OnEnded (object sender, EventArgs eventArgs)
        {
            if (Control.Value != Element.Text)
                ElementController.SetValueFromRenderer (Editor.TextProperty, Control.Value);

            Element.SetValue (VisualElement.IsFocusedPropertyKey, false);
            ElementController.SendCompleted ();
        }

        void OnStarted (object sender, EventArgs eventArgs)
        {
            ElementController.SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, true);
        }

        void UpdateEditable ()
        {
            Control.IsDisabled = !Element.IsEnabled;
        }

        void UpdateFont ()
        {
            Element.SetStyleFont (Element.FontFamily, Element.FontSize, Element.FontAttributes, Control.Style);
        }

        void UpdateKeyboard ()
        {
        }

        void UpdateText ()
        {
            if (Control.Value != Element.Text)
                Control.Value = Element.Text;
        }

        void UpdateTextAlignment ()
        {
        }

        void UpdateTextColor ()
        {
            Control.Style.Color = Element.TextColor.ToOouiColor (OouiTheme.TextColor);
        }
    }
}
