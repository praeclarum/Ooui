using Ooui.Forms.Extensions;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class EditorRenderer : ViewRenderer<Editor, Ooui.TextArea>
    {
        private bool _disposed;
        private Ooui.Color _defaultTextColor;

        static Size initialSize = Size.Zero;

        protected IElementController ElementController => Element as IElementController;

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            var size = Element.Text.MeasureSize(Element.FontFamily, Element.FontSize, Element.FontAttributes);
            size = new Size(size.Width, size.Height * 1.428 + 14);

            return new SizeRequest(size, size);
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
                    Control.Inputted -= OnEditingChanged;
                    Control.Changed -= OnEditingEnded;
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            if (e.NewElement == null)
                return;

            if (Control == null)
            {
                SetNativeControl(new Ooui.TextArea());

                _defaultTextColor = Colors.Black;

                Debug.Assert(Control != null, "Control != null");

                Control.Inputted += OnEditingChanged;
                Control.Changed += OnEditingEnded;
            }

            UpdateText();
            UpdateTextColor();
            UpdateFont();

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Editor.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Editor.TextColorProperty.PropertyName)
                UpdateTextColor();
            else if (e.PropertyName == Editor.FontAttributesProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == Editor.FontFamilyProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == Editor.FontSizeProperty.PropertyName)
                UpdateFont();
        }

        void UpdateText()
        {
            if (Control.Value != Element.Text)
                Control.Value = Element.Text;
        }

        void UpdateTextColor()
        {
            var textColor = Element.TextColor;

            if (textColor.IsDefault || !Element.IsEnabled)
                Control.Style.Color = _defaultTextColor;
            else
                Control.Style.Color = textColor.ToOouiColor();
        }

        void UpdateFont()
        {
            if (initialSize == Size.Zero)
            {
                var testString = "Tj";
                initialSize = testString.MeasureSize(Control.Style);
            }

            Element.SetStyleFont(Element.FontFamily, Element.FontSize, Element.FontAttributes, Control.Style);
        }

        private void OnEditingChanged(object sender, TargetEventArgs e)
        {
            ElementController.SetValueFromRenderer(Editor.TextProperty, Control.Value);
        }

        private void OnEditingEnded(object sender, TargetEventArgs e)
        {
            if (Control.Text != Element.Text)
            {
                ElementController.SetValueFromRenderer(Editor.TextProperty, Control.Text);
            }
        }
    }
}
