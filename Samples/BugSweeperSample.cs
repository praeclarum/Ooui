using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class BugSweeperSample : ISample
    {
        public string Title => "Xamarin.Forms BugSweeper";

        public Ooui.Html.Element CreateElement ()
        {
            var page = new BugSweeper.BugSweeperPage ();
            return page.GetOouiElement ();
        }
    }
}
