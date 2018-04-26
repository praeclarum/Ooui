using System;
using System.ComponentModel;
using Xamarin.Forms;
using Ooui.Forms.Extensions;

namespace Ooui.Forms.Renderers
{
    public class FrameRenderer : VisualElementRenderer<Frame>
    {
        protected override void OnElementChanged (ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged (e);

            if (e.NewElement != null)
                SetupLayer ();
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);

            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
                e.PropertyName == Xamarin.Forms.Frame.OutlineColorProperty.PropertyName ||
                e.PropertyName == Xamarin.Forms.Frame.HasShadowProperty.PropertyName ||
                e.PropertyName == Xamarin.Forms.Frame.CornerRadiusProperty.PropertyName)
                SetupLayer ();
        }

        void SetupLayer ()
        {
            float cornerRadius = Element.CornerRadius;

            if (cornerRadius == -1f)
                cornerRadius = 5f; // default corner radius

            var Layer = this.Style;

            Layer.BorderRadius = cornerRadius;

            Layer.BackgroundColor = Element.BackgroundColor.ToOouiColor (OouiTheme.BackgroundColor);

            if (Element.HasShadow) {
                //Layer.ShadowRadius = 5;
                //Layer.ShadowColor = "black";
                //Layer.ShadowOpacity = 0.8f;
                //Layer.ShadowOffset = new SizeF ();
            }
            else {
                //Layer.ShadowOpacity = 0;
            }

            if (Element.OutlineColor == Xamarin.Forms.Color.Default) {
                Layer.BorderColor = Colors.Clear;
                Layer.BorderWidth = 1;
                Layer.BorderStyle = "none";
            }
            else {
                Layer.BorderColor = Element.OutlineColor.ToOouiColor (Colors.Clear);
                Layer.BorderWidth = 1;
                Layer.BorderStyle = "solid";
            }
        }
    }
}
