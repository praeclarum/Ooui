using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class DotMatrixClockSample : ISample
    {
        public string Title => "Xamarin.Forms DoMatrixClock";

        public Ooui.Html.Element CreateElement()
        {
            var page = new DotMatrixClock.DotMatrixClockPage();
            return page.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish("/dotmatrixclock", CreateElement);
        }
    }
}
