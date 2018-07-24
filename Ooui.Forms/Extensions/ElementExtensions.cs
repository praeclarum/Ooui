using System;
using Xamarin.Forms;

namespace Ooui.Forms.Extensions
{
    public static class ElementExtensions
    {
        public static SizeRequest GetSizeRequest (this Ooui.Html.Element self, double widthConstraint, double heightConstraint,
            double minimumWidth = -1, double minimumHeight = -1)
        {
            var rw = 0.0;
            var rh = 0.0;
            Size s = new Size (0, 0);
            var measured = false;

            if (self.Style.Width.Equals ("inherit")) {
                s = self.Text.MeasureSize (self.Style, widthConstraint, heightConstraint);
                measured = true;
                rw = double.IsPositiveInfinity (s.Width) ? double.PositiveInfinity : Math.Ceiling (s.Width);
            }
            else {
                rw = self.Style.GetNumberWithUnits ("width", "px", 640);
            }

            if (self.Style.Height.Equals ("inherit")) {
                if (!measured) {
                    s = self.Text.MeasureSize (self.Style, widthConstraint, heightConstraint);
                    measured = true;
                }
                rh = double.IsPositiveInfinity (s.Height) ? double.PositiveInfinity : Math.Ceiling (s.Height * 1.4);
            }
            else {
                rh = self.Style.GetNumberWithUnits ("height", "px", 480);
            }

            var minimum = new Size (minimumWidth < 0 ? rw : minimumWidth,
                minimumHeight < 0 ? rh : minimumHeight);

            return new SizeRequest (new Size (rw, rh), minimum);
        }
    }
}
