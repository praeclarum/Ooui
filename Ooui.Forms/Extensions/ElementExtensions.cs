using System;
using Xamarin.Forms;

namespace Ooui.Forms.Extensions
{
    public static class ElementExtensions
    {
        public static SizeRequest GetSizeRequest (this Ooui.Element self, double widthConstraint, double heightConstraint,
            double minimumWidth = -1, double minimumHeight = -1)
        {
            var request = new Size (double.PositiveInfinity, double.PositiveInfinity);
            var minimum = new Size (double.PositiveInfinity, double.PositiveInfinity);
            return new SizeRequest (request, minimum);
        }
    }
}
