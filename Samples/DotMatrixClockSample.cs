using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class DotMatrixClockSample : ISample
    {
        public string Title => "Xamarin.Forms DoMatrixClock";
        public string Path => "/dotmatrixclock";

        public Ooui.Element CreateElement()
        {
            var page = new DotMatrixClock.DotMatrixClockPage();
            return page.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish(Path, CreateElement);
        }
    }
}
