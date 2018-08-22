using System;
using System.ComponentModel;
using Ooui.Forms.Extensions;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public abstract class ViewRenderer : ViewRenderer<View, Ooui.Html.Element>
    {
    }

	public class ViewRenderer<TElement, TNativeElement> : VisualElementRenderer<TElement> where TElement : View where TNativeElement : Ooui.Html.Element
	{
		public TNativeElement Control { get; private set; }

        /// <summary>
        /// Determines whether the native control is disposed of when this renderer is disposed
        /// Can be overridden in deriving classes 
        /// </summary>
        protected virtual bool ManageNativeControlLifetime => true;

		protected override bool HtmlNeedsFullEndElement => TagName == "div";

        public ViewRenderer (string tagName = "div")
            : base (tagName)
        {
        }

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			if (disposing && Control != null && ManageNativeControlLifetime) {
				Control = null;
			}
		}

		protected override void OnElementChanged (ElementChangedEventArgs<TElement> e)
		{
			base.OnElementChanged (e);

			if (e.OldElement != null)
				e.OldElement.FocusChangeRequested -= ViewOnFocusChangeRequested;

			if (e.NewElement != null) {
				if (Control != null && e.OldElement != null && e.OldElement.BackgroundColor != e.NewElement.BackgroundColor || e.NewElement.BackgroundColor != Xamarin.Forms.Color.Default)
					SetBackgroundColor (e.NewElement.BackgroundColor);

				e.NewElement.FocusChangeRequested += ViewOnFocusChangeRequested;
			}

			UpdateIsEnabled ();
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (Control != null) {
				if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
					UpdateIsEnabled ();
				else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
					SetBackgroundColor (Element.BackgroundColor);
			}

			base.OnElementPropertyChanged (sender, e);
		}

		protected override void OnRegisterEffect (PlatformEffect effect)
		{
			base.OnRegisterEffect (effect);
			//effect.Control = Control;
		}

		protected override void SetAutomationId (string id)
		{
			if (Control == null)
				base.SetAutomationId (id);
			else {
			}
		}

		protected override void SetBackgroundColor (Xamarin.Forms.Color color)
		{
			if (Control == null)
				return;

            Control.Style.BackgroundColor = color.ToOouiColor (OouiTheme.BackgroundColor);
		}

        protected void SetNativeControl (Ooui.Html.Element element)
		{
			Control = (TNativeElement)element;

			if (Element.BackgroundColor != Xamarin.Forms.Color.Default)
				SetBackgroundColor (Element.BackgroundColor);

			UpdateIsEnabled ();

			this.AppendChild (element);
		}

        public override void SetControlSize (Size size)
        {
            if (Control != null) {
                Control.Style.Width = size.Width;
                Control.Style.Height = size.Height;
            }
        }

		protected override void SendVisualElementInitialized (VisualElement element, Ooui.Html.Element nativeView)
		{
			base.SendVisualElementInitialized (element, Control);
		}

		void UpdateIsEnabled ()
		{
			if (Element == null || Control == null)
				return;

			var uiControl = Control as Ooui.Html.FormControl;
			if (uiControl == null)
				return;
			uiControl.IsDisabled = !Element.IsEnabled;
		}

		void ViewOnFocusChangeRequested (object sender, VisualElement.FocusRequestArgs focusRequestArgs)
		{
		}
	}
}
