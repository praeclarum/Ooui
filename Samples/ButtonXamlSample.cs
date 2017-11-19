using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class ButtonXamlSample : ISample
    {
        public string Title => "Xamarin.Forms Button XAML";

        public Ooui.Element CreateElement ()
        {
            var page = new ButtonXaml.ButtonXamlPage ();
            return page.GetOouiElement ();
        }
    }
}
