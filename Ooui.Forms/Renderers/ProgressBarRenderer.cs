using System;
using System.ComponentModel;
using System.Linq;

#if MAUI
using Microsoft.Maui.Controls;
#else
using Xamarin.Forms;
#endif

namespace Ooui.Forms.Renderers
{
    public class ProgressBarRenderer : ViewRenderer<ProgressBar, Div>
    {
        public override SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
        {
            var size = new Size (80, 20);
            return new SizeRequest (size, size);
        }

        protected override void OnElementChanged (ElementChangedEventArgs<ProgressBar> e)
        {
            if (e.NewElement != null) {
                if (Control == null) {
                    var p = new Div { ClassName = "progress" };
                    var pb = new Div { ClassName = "progress-bar progress-bar" };
                    pb.SetAttribute ("role", "progressbar");
                    pb.SetAttribute ("aria-valuenow", "0");
                    pb.SetAttribute ("aria-valuemin", "0");
                    pb.SetAttribute ("aria-valuemax", "100");
                    pb.Style.Width = "0%";
                    p.AppendChild (pb);
                    SetNativeControl (p);
                }

                UpdateProgress ();
            }

            base.OnElementChanged (e);
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);

            if (e.PropertyName == ProgressBar.ProgressProperty.PropertyName)
                UpdateProgress ();
        }

        void UpdateProgress ()
        {
            var pb = Control?.Children.FirstOrDefault () as Div;
            pb.Style.Width = string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}%", Element.Progress*100);
        }
    }
}
