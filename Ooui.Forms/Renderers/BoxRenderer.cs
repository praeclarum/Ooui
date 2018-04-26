using System;
using System.ComponentModel;
using Ooui.Forms.Extensions;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class BoxRenderer : VisualElementRenderer<BoxView>
    {
        Ooui.Color _colorToRenderer;

        protected override void OnElementChanged (ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged (e);

            if (Element != null)
                SetBackgroundColor (Element.BackgroundColor);
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);
            if (e.PropertyName == BoxView.ColorProperty.PropertyName)
                SetBackgroundColor (Element.BackgroundColor);
        }

        protected override void SetBackgroundColor (Xamarin.Forms.Color color)
        {
            if (Element == null)
                return;

            _colorToRenderer = Element.Color.ToOouiColor (Colors.Clear);

            Style.BackgroundColor = _colorToRenderer;
        }
    }
}
