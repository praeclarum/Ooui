﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using Ooui.Forms.Extensions;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class ButtonRenderer : ViewRenderer<Xamarin.Forms.Button, Ooui.Html.Button>
    {
        Ooui.Color _buttonTextColorDefaultDisabled;
        Ooui.Color _buttonTextColorDefaultHighlighted;
        Ooui.Color _buttonTextColorDefaultNormal;

        public override SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
        {
            var size = Element.Text.MeasureSize (Element.FontFamily, Element.FontSize, Element.FontAttributes, widthConstraint, heightConstraint);
            size = new Size (size.Width + 32, size.Height + 16);
            return new SizeRequest (size, size);
        }

        protected override void Dispose (bool disposing)
        {
            if (Control != null) {
                Control.Click -= OnButtonTouchUpInside;
            }

            base.Dispose (disposing);
        }

        protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged (e);

            if (e.NewElement != null) {
                if (Control == null) {
                    SetNativeControl (new Ooui.Html.Button ());

                    Debug.Assert (Control != null, "Control != null");

                    Control.ClassName = "btn btn-primary";

                    _buttonTextColorDefaultNormal = Ooui.Colors.Black;
                    _buttonTextColorDefaultHighlighted = Ooui.Colors.Black;
                    _buttonTextColorDefaultDisabled = Ooui.Colors.Black;

                    Control.Click += OnButtonTouchUpInside;
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
            else if (e.PropertyName == Xamarin.Forms.Button.BorderWidthProperty.PropertyName || e.PropertyName == Xamarin.Forms.Button.CornerRadiusProperty.PropertyName || e.PropertyName == Xamarin.Forms.Button.BorderColorProperty.PropertyName)
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
                uiButton.Style.BorderColor = button.BorderColor.ToOouiColor (OouiTheme.ButtonBorderColor);
            else
                uiButton.Style.BorderColor = null;

            float bw = Math.Max (0f, (float)button.BorderWidth);
            if (bw > 0) {
                uiButton.Style.BorderWidth = bw + "px";
            }
            else {
                uiButton.Style.BorderWidth = null;
            }

            var br = button.CornerRadius;
            if (br > 0 && (bw > 0 || br != 5)) { // 5 is the default
                uiButton.Style.BorderRadius = br + "px";
            }
            else {
                uiButton.Style.BorderRadius = null;
            }
        }

        void UpdateFont ()
        {
            Element.SetStyleFont (Element.FontFamily, Element.FontSize, Element.FontAttributes, Control.Style);
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
            Control.Style.Color = Element.TextColor.ToOouiColor (OouiTheme.ButtonTextColor);
        }
    }
}
