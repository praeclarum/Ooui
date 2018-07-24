using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Ooui.Html;

namespace Ooui.Forms.Renderers
{
    public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, Div>
    {
        public override SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
        {
            var size = new Size (40, 20);
            return new SizeRequest (size, size);
        }

        protected override void OnElementChanged (ElementChangedEventArgs<ActivityIndicator> e)
        {
            if (e.NewElement != null) {
                if (Control == null) {
                    var p = new Div { ClassName = "progress" };
                    var pb = new Div { ClassName = "progress-bar progress-bar-striped" };
                    pb.SetAttribute ("role", "progressbar");
                    pb.SetAttribute ("aria-valuenow", "0");
                    pb.SetAttribute ("aria-valuemin", "0");
                    pb.SetAttribute ("aria-valuemax", "100");
                    pb.Style.Width = "0%";
                    p.AppendChild (pb);
                    SetNativeControl (p);
                }

                UpdateColor ();
                UpdateIsRunning ();
            }

            base.OnElementChanged (e);
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);

            if (e.PropertyName == ActivityIndicator.ColorProperty.PropertyName)
                UpdateColor ();
            else if (e.PropertyName == ActivityIndicator.IsRunningProperty.PropertyName)
                UpdateIsRunning ();
        }

        void UpdateColor ()
        {
        }

        void UpdateIsRunning ()
        {
            var pb = (Div)Control.Children[0];
            if (Element.IsRunning) {
                pb.SetAttribute ("aria-valuenow", "100");
                pb.Style.Width = "100%";
                pb.ClassName = "progress-bar progress-bar-striped active";
            }
            else {
                pb.SetAttribute ("aria-valuenow", "0");
                pb.Style.Width = "0%";
                pb.ClassName = "progress-bar progress-bar-striped";
            }
        }
    }
}
