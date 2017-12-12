using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class XuzzleSample : ISample
    {
        public string Title => "Xamarin.Forms Xuzzle";

        public Ooui.Element CreateElement()
        {
            var page = new Xuzzle.XuzzlePage();
            return page.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish("/xuzzle", CreateElement);
        }
    }
}
