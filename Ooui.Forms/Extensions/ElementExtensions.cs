using System;
using Xamarin.Forms;

namespace Ooui.Forms.Extensions
{
    public static class ElementExtensions
    {
        public static SizeRequest GetSizeRequest (this Ooui.Element self, double widthConstraint, double heightConstraint,
            double minimumWidth = -1, double minimumHeight = -1)
        {
            var s = self.Text.MeasureSize (self.Style);

            var request = new Size (
                double.IsPositiveInfinity (s.Width) ? double.PositiveInfinity : s.Width,
                double.IsPositiveInfinity (s.Height) ? double.PositiveInfinity : s.Height);
            var minimum = new Size (minimumWidth < 0 ? request.Width : minimumWidth,
                minimumHeight < 0 ? request.Height : minimumHeight);

            return new SizeRequest (request, minimum);
        }
    }
}
