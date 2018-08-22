using System;
using System.ComponentModel;
using Ooui.Forms.Extensions;
using Ooui.Html;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class LinkLabelRenderer : ViewRenderer<LinkLabel, Anchor>
    {
        SizeRequest _perfectSize;

        bool _perfectSizeValid;

        public override SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
        {
            if (!_perfectSizeValid) {
                var size = Element.Text.MeasureSize (Element.FontFamily, Element.FontSize, Element.FontAttributes, double.PositiveInfinity, double.PositiveInfinity);
                size.Width = Math.Ceiling (size.Width);
                size.Height = Math.Ceiling (size.Height);
                _perfectSize = new SizeRequest (size, new Size (Element.FontSize, Element.FontSize));
                _perfectSizeValid = true;
            }

            var widthFits = widthConstraint >= _perfectSize.Request.Width;
            var heightFits = heightConstraint >= _perfectSize.Request.Height;

            if (widthFits && heightFits)
                return _perfectSize;

            var resultRequestSize = Element.Text.MeasureSize (Element.FontFamily, Element.FontSize, Element.FontAttributes, widthConstraint, heightConstraint);
            var result = new SizeRequest (resultRequestSize, resultRequestSize);
            var tinyWidth = Math.Min (10, result.Request.Width);
            result.Minimum = new Size (tinyWidth, result.Request.Height);

            if (widthFits || Element.LineBreakMode == LineBreakMode.NoWrap)
                return result;

            bool containerIsNotInfinitelyWide = !double.IsInfinity (widthConstraint);

            if (containerIsNotInfinitelyWide) {
                bool textCouldHaveWrapped = Element.LineBreakMode == LineBreakMode.WordWrap || Element.LineBreakMode == LineBreakMode.CharacterWrap;
                bool textExceedsContainer = result.Request.Width > widthConstraint;

                if (textExceedsContainer || textCouldHaveWrapped) {
                    var expandedWidth = Math.Max (tinyWidth, widthConstraint);
                    result.Request = new Size (expandedWidth, result.Request.Height);
                }
            }

            return result;
        }

        protected override void OnElementChanged (ElementChangedEventArgs<Ooui.Forms.LinkLabel> e)
        {
            if (e.NewElement != null) {
                if (Control == null) {
                    SetNativeControl (new Anchor ());
                }

                UpdateHRef ();
                UpdateTarget ();

                UpdateText ();
                UpdateTextColor ();
                UpdateFont ();

                UpdateLineBreakMode ();
                UpdateAlignment ();
            }

            base.OnElementChanged (e);
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);

            if (Control == null)
                return;

            if (e.PropertyName == Ooui.Forms.LinkLabel.HRefProperty.PropertyName)
                UpdateHRef ();
            if (e.PropertyName == Ooui.Forms.LinkLabel.TargetProperty.PropertyName)
                UpdateTarget ();
            else if (e.PropertyName == Xamarin.Forms.Label.HorizontalTextAlignmentProperty.PropertyName)
                UpdateAlignment ();
            else if (e.PropertyName == Xamarin.Forms.Label.VerticalTextAlignmentProperty.PropertyName)
                UpdateAlignment ();
            else if (e.PropertyName == Xamarin.Forms.Label.TextColorProperty.PropertyName)
                UpdateTextColor ();
            else if (e.PropertyName == Xamarin.Forms.Label.FontProperty.PropertyName)
                UpdateFont ();
            else if (e.PropertyName == Xamarin.Forms.Label.TextProperty.PropertyName)
                UpdateText ();
            else if (e.PropertyName == Xamarin.Forms.Label.FormattedTextProperty.PropertyName)
                UpdateText ();
            else if (e.PropertyName == Xamarin.Forms.Label.LineBreakModeProperty.PropertyName)
                UpdateLineBreakMode ();
        }

        protected override void SetBackgroundColor (Xamarin.Forms.Color color)
        {
            Style.BackgroundColor = color.ToOouiColor (Colors.Clear);
        }

        void UpdateHRef ()
        {
            Control.HRef = Element.HRef;
        }

        void UpdateTarget ()
        {
            Control.Target = Element.Target;
        }

        void UpdateAlignment ()
        {
            this.Style.Display = "table";
            Control.Style.Display = "table-cell";
            this.Style.TextAlign = Element.HorizontalTextAlignment.ToOouiTextAlign ();
            Control.Style.VerticalAlign = Element.VerticalTextAlignment.ToOouiVerticalAlign ();
        }

        void UpdateLineBreakMode ()
        {
            _perfectSizeValid = false;
        }

        bool isTextFormatted;
        void UpdateText ()
        {
            _perfectSizeValid = false;

            var values = Element.GetValues (Xamarin.Forms.Label.FormattedTextProperty, Xamarin.Forms.Label.TextProperty, Xamarin.Forms.Label.TextColorProperty);
            var formatted = values[0] as FormattedString;
            if (formatted != null) {
                Control.Text = (string)values[1];
                isTextFormatted = true;
            }
            else {
                Control.Text = (string)values[1];
                isTextFormatted = false;
            }
        }

        void UpdateFont ()
        {
            if (isTextFormatted)
                return;
            _perfectSizeValid = false;

            Element.SetStyleFont (Element.FontFamily, Element.FontSize, Element.FontAttributes, Control.Style);
        }

        void UpdateTextColor ()
        {
            if (isTextFormatted)
                return;

            _perfectSizeValid = false;

            var textColor = Element.TextColor;
            if (textColor.IsDefault) {
                Control.Style.Color = null;
            }
            else {
                Control.Style.Color = textColor.ToOouiColor (Xamarin.Forms.Color.Black);
            }
        }
    }
}
