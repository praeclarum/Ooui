using System;
using System.ComponentModel;
using System.Diagnostics;
using Ooui.Forms.Extensions;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class ButtonRenderer : ViewRenderer<Xamarin.Forms.Button, Ooui.Button>
    {
        Ooui.Color _buttonTextColorDefaultDisabled;
        Ooui.Color _buttonTextColorDefaultHighlighted;
        Ooui.Color _buttonTextColorDefaultNormal;

        protected override void Dispose (bool disposing)
        {
            if (Control != null) {
                Control.Clicked -= OnButtonTouchUpInside;
            }

            base.Dispose (disposing);
        }

        protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged (e);

            if (e.NewElement != null) {
                if (Control == null) {
                    SetNativeControl (new Ooui.Button ());

                    Debug.Assert (Control != null, "Control != null");

                    _buttonTextColorDefaultNormal = Ooui.Colors.Black;
                    _buttonTextColorDefaultHighlighted = Ooui.Colors.Black;
                    _buttonTextColorDefaultDisabled = Ooui.Colors.Black;

                    Control.Clicked += OnButtonTouchUpInside;
                }

                UpdateText ();
                UpdateFont ();
                UpdateBorder ();
                UpdateImage ();
                UpdateTextColor ();
            }
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);

            if (e.PropertyName == Xamarin.Forms.Button.TextProperty.PropertyName)
                UpdateText ();
            else if (e.PropertyName == Xamarin.Forms.Button.TextColorProperty.PropertyName)
                UpdateTextColor ();
            else if (e.PropertyName == Xamarin.Forms.Button.FontProperty.PropertyName)
                UpdateFont ();
            else if (e.PropertyName == Xamarin.Forms.Button.BorderWidthProperty.PropertyName || e.PropertyName == Xamarin.Forms.Button.BorderRadiusProperty.PropertyName || e.PropertyName == Xamarin.Forms.Button.BorderColorProperty.PropertyName)
                UpdateBorder ();
            else if (e.PropertyName == Xamarin.Forms.Button.ImageProperty.PropertyName)
                UpdateImage ();
        }

        void OnButtonTouchUpInside (object sender, EventArgs eventArgs)
        {
            ((IButtonController)Element)?.SendPressed ();
            ((IButtonController)Element)?.SendReleased ();
            ((IButtonController)Element)?.SendClicked ();
        }

        void UpdateBorder ()
        {
            var uiButton = Control;
            var button = Element;

            if (button.BorderColor != Xamarin.Forms.Color.Default)
                uiButton.Style.BorderColor = button.BorderColor.ToOouiColor ();

            uiButton.Style.BorderWidth = Math.Max (0f, (float)button.BorderWidth);
            uiButton.Style.BorderRadius = button.BorderRadius;
        }

        void UpdateFont ()
        {
            Element.SetStyleFont (Control.Style);
        }

        void UpdateImage ()
        {
            //IImageSourceHandler handler;
            //FileImageSource source = Element.Image;
            //if (source != null && (handler = Internals.Registrar.Registered.GetHandlerForObject<IImageSourceHandler> (source)) != null) {
            //    UIImage uiimage;
            //    try {
            //        uiimage = await handler.LoadImageAsync (source, scale: (float)UIScreen.MainScreen.Scale);
            //    }
            //    catch (OperationCanceledException) {
            //        uiimage = null;
            //    }
            //    Ooui.Button button = Control;
            //    if (button != null && uiimage != null) {
            //        button.SetImage (uiimage.ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);

            //        button.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            //        ComputeEdgeInsets (Control, Element.ContentLayout);
            //    }
            //}
            //else {
            //    Control.SetImage (null, UIControlState.Normal);
            //    ClearEdgeInsets (Control);
            //}
            //((IVisualElementController)Element).NativeSizeChanged ();
        }

        void UpdateText ()
        {
            var newText = Element.Text;

            if (Control.Text != newText) {
                Control.Text = Element.Text;
            }
        }

        void UpdateTextColor ()
        {
            if (Element.TextColor == Xamarin.Forms.Color.Default) {
                Control.Style.Color = _buttonTextColorDefaultNormal;
                Control.Style.Color = _buttonTextColorDefaultHighlighted;
                Control.Style.Color = _buttonTextColorDefaultDisabled;
            }
            else {
                Control.Style.Color = Element.TextColor.ToOouiColor ();
                Control.Style.Color = Element.TextColor.ToOouiColor ();
                Control.Style.Color = _buttonTextColorDefaultDisabled;
            }
        }
    }
}
